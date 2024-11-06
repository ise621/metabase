import { message, List, Button } from "antd";
import { useEffect, useState } from "react";
import {
  DatabaseDocument,
  DatabasesDocument,
  PendingDatabasesDocument,
  usePendingDatabasesQuery,
  useVerifyDatabaseMutation,
} from "../../queries/databases.graphql";
import { Scalars } from "../../__generated__/__types__";
import Link from "next/link";
import paths from "../../paths";
import { messageApolloError } from "../../lib/apollo";

export type PendingDatabasesProps = {};

export default function PendingDatabases({}: PendingDatabasesProps) {
  const { data, loading, error } = usePendingDatabasesQuery();

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  const [verifyDatabaseMutation] = useVerifyDatabaseMutation();
  const [verifyingDatabase, setVerifyingDatabase] = useState(false);

  const verifyDatabase = async (databaseId: Scalars["Uuid"]) => {
    try {
      setVerifyingDatabase(true);
      const { errors, data } = await verifyDatabaseMutation({
        variables: {
          databaseId: databaseId,
        },
        refetchQueries: [
          {
            query: DatabasesDocument,
          },
          {
            query: PendingDatabasesDocument,
          },
          {
            query: DatabaseDocument,
            variables: {
              uuid: databaseId,
            },
          },
        ],
      });
      if (errors) {
        console.log(errors); // TODO What to do?
      } else if (data?.verifyDatabase?.errors) {
        // TODO Is this how we want to display errors?
        message.error(
          data?.verifyDatabase?.errors.map((error) => error.message).join(" ")
        );
      }
    } finally {
      setVerifyingDatabase(false);
    }
  };

  return (
    <List
      size="small"
      loading={loading}
      dataSource={data?.pendingDatabases?.edges?.map((e) => e.node) || []}
      renderItem={(item) => (
        <List.Item>
          <Link href={paths.database(item?.uuid)} legacyBehavior>{item?.name}</Link>
          {item.canCurrentUserVerifyNode && (
            <Button
              onClick={() => verifyDatabase(item?.uuid)}
              loading={verifyingDatabase}
            >
              Verify
            </Button>
          )}
        </List.Item>
      )}
    />
  );
}
