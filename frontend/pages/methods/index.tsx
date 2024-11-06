import { messageApolloError } from "../../lib/apollo";
import Layout from "../../components/Layout";
import { Typography } from "antd";
import { useMethodsQuery } from "../../queries/methods.graphql";
import { useEffect } from "react";
import paths from "../../paths";
import Link from "next/link";
import MethodTable from "../../components/methods/MethodTable";

// TODO Pagination. See https://www.apollographql.com/docs/react/pagination/core-api/

function Page() {
  const { loading, error, data } = useMethodsQuery();
  const nodes = data?.methods?.edges?.map((e) => e.node) || [];

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  return (
    <Layout>
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        <Link href={paths.data}>Data</Link> is created by applying a method.
        Methods can be defined for example by a standard.
      </Typography.Paragraph>
      <MethodTable loading={loading} methods={nodes} />
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The{" "}
        <Typography.Link
          href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
        >
          GraphQL endpoint
        </Typography.Link>{" "}
        provides all information about methods.
      </Typography.Paragraph>
    </Layout>
  );
}

export default Page;
