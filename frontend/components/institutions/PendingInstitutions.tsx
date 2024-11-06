import { message, List, Button } from "antd";
import { useEffect, useState } from "react";
import {
  InstitutionDocument,
  InstitutionsDocument,
  PendingInstitutionsDocument,
  usePendingInstitutionsQuery,
  useVerifyInstitutionMutation,
} from "../../queries/institutions.graphql";
import { Scalars } from "../../__generated__/__types__";
import Link from "next/link";
import paths from "../../paths";
import { messageApolloError } from "../../lib/apollo";

export type PendingInstitutionsProps = {};

export default function PendingInstitutions({}: PendingInstitutionsProps) {
  const { data, loading, error } = usePendingInstitutionsQuery();

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  const [verifyInstitutionMutation] = useVerifyInstitutionMutation();
  const [verifyingInstitution, setVerifyingInstitution] = useState(false);

  const verifyInstitution = async (institutionId: Scalars["Uuid"]) => {
    try {
      setVerifyingInstitution(true);
      const { errors, data } = await verifyInstitutionMutation({
        variables: {
          institutionId: institutionId,
        },
        refetchQueries: [
          {
            query: InstitutionsDocument,
          },
          {
            query: PendingInstitutionsDocument,
          },
          {
            query: InstitutionDocument,
            variables: {
              uuid: institutionId,
            },
          },
        ],
      });
      if (errors) {
        console.log(errors); // TODO What to do?
      } else if (data?.verifyInstitution?.errors) {
        // TODO Is this how we want to display errors?
        message.error(
          data?.verifyInstitution?.errors
            .map((error) => error.message)
            .join(" ")
        );
      }
    } finally {
      setVerifyingInstitution(false);
    }
  };

  return (
    <List
      size="small"
      loading={loading}
      dataSource={data?.pendingInstitutions?.edges?.map((e) => e.node) || []}
      renderItem={(item) => (
        <List.Item>
          <Link href={paths.institution(item?.uuid)} legacyBehavior>{item?.name}</Link>
          <Button
            onClick={() => verifyInstitution(item?.uuid)}
            loading={verifyingInstitution}
          >
            Verify
          </Button>
        </List.Item>
      )}
    />
  );
}
