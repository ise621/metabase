import Layout from "../../components/Layout";
import { Table, Typography } from "antd";
import { useUsersQuery } from "../../queries/users.graphql";
import paths from "../../paths";
import { useEffect, useState } from "react";
import { setMapValue } from "../../lib/freeTextFilter";
import {
  getFilterableStringColumnProps,
  getNameColumnProps,
  getUuidColumnProps,
} from "../../lib/table";
import Link from "next/link";
import { messageApolloError } from "../../lib/apollo";

// TODO Pagination. See https://www.apollographql.com/docs/react/pagination/core-api/

function Page() {
  const { loading, error, data } = useUsersQuery();
  const nodes = data?.users?.edges?.map((e) => e.node) || [];

  const [filterText, setFilterText] = useState(() => new Map<string, string>());
  const onFilterTextChange = setMapValue(filterText, setFilterText);

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  return (
    <Layout>
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        A user is usually affiliated to an{" "}
        <Link href={paths.institutions}>institution</Link>. In further steps,
        users can receive the permission for example to add{" "}
        <Link href={paths.components}>components</Link> manufactured by this{" "}
        <Link href={paths.institutions}>institution</Link>.
      </Typography.Paragraph>
      <Table
        loading={loading}
        columns={[
          getUuidColumnProps<(typeof nodes)[0]>(
            onFilterTextChange,
            (x) => filterText.get(x),
            paths.user
          ),
          getNameColumnProps<(typeof nodes)[0]>(onFilterTextChange, (x) =>
            filterText.get(x)
          ),
          getFilterableStringColumnProps<(typeof nodes)[0]>(
            "Email",
            "email",
            (record) => record.email,
            onFilterTextChange,
            (x) => filterText.get(x)
          ),
        ]}
        dataSource={nodes}
      />
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The{" "}
        <Typography.Link
          href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
        >
          GraphQL endpoint
        </Typography.Link>{" "}
        can as well be used to find, for example, the users of your{" "}
        <Link href={paths.institutions}>institution</Link>.
      </Typography.Paragraph>
    </Layout>
  );
}

export default Page;
