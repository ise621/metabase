import { messageApolloError } from "../../lib/apollo";
import Layout from "../../components/Layout";
import paths from "../../paths";
import { Divider, Table, Typography } from "antd";
import { useDatabasesQuery } from "../../queries/databases.graphql";
import { useEffect, useState } from "react";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import { setMapValue } from "../../lib/freeTextFilter";
import PendingDatabases from "../../components/databases/PendingDatabases";
import {
  getExternallyLinkedFilterableLocatorColumnProps,
  getNameColumnProps,
  getDescriptionColumnProps,
  getInternallyLinkedFilterableStringColumnProps,
  getUuidColumnProps,
} from "../../lib/table";
import Link from "next/link";
import { UserRole } from "../../__generated__/__types__";

// TODO Pagination. See https://www.apollographql.com/docs/react/pagination/core-api/

function Page() {
  const { loading, error, data } = useDatabasesQuery();
  const nodes = data?.databases?.edges?.map((e) => e.node) || [];

  const [filterText, setFilterText] = useState(() => new Map<string, string>());
  const onFilterTextChange = setMapValue(filterText, setFilterText);

  const currentUser = useCurrentUserQuery()?.data?.currentUser;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  return (
    <Layout>
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The following databases are connected to{" "}
        <Link href={paths.home}>buildingenvelopedata.org</Link> and contain{" "}
        <Link href={paths.data}>data</Link> on{" "}
        <Link href={paths.components}>components</Link>.
      </Typography.Paragraph>
      <Table
        loading={loading}
        columns={[
          {
            ...getUuidColumnProps<(typeof nodes)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
              paths.database
            ),
          },
          {
            ...getNameColumnProps<(typeof nodes)[0]>(onFilterTextChange, (x) =>
              filterText.get(x)
            ),
          },
          {
            ...getDescriptionColumnProps<(typeof nodes)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x)
            ),
          },
          {
            ...getExternallyLinkedFilterableLocatorColumnProps<
              (typeof nodes)[0]
            >(
              "Locator",
              "locator",
              (record) => record.locator,
              onFilterTextChange,
              (x) => filterText.get(x)
            ),
          },
          {
            ...getInternallyLinkedFilterableStringColumnProps<
              (typeof nodes)[0]
            >(
              "Operator",
              "operator",
              (record) => record.operator.node.name,
              onFilterTextChange,
              (x) => filterText.get(x),
              (x) => paths.institution(x.operator.node.uuid)
            ),
          },
        ]}
        dataSource={nodes}
      />
      {currentUser && currentUser?.roles?.includes(UserRole.Administrator) && (
        <>
          <Divider />
          <Typography.Title level={2}>Pending Databases</Typography.Title>
          <PendingDatabases />
          <Divider />
        </>
      )}
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The{" "}
        <Typography.Link
          href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
        >
          GraphQL endpoint
        </Typography.Link>{" "}
        provides all information about databases.
      </Typography.Paragraph>
    </Layout>
  );
}

export default Page;
