import { messageApolloError } from "../../lib/apollo";
import Layout from "../../components/Layout";
import { Typography } from "antd";
import { useComponentsQuery } from "../../queries/components.graphql";
import { useEffect } from "react";
import paths from "../../paths";
import Link from "next/link";
import { ComponentTable } from "../../components/components/ComponentTable";

// TODO Pagination. See https://www.apollographql.com/docs/react/pagination/core-api/

function Page() {
  const { loading, error, data } = useComponentsQuery();
  const nodes = data?.components?.edges?.map((e) => e.node) || [];

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  return (
    <Layout>
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The building envelope components for which{" "}
        <Link href={paths.data}>data</Link> is available are presented here.
      </Typography.Paragraph>
      <ComponentTable loading={loading} components={nodes} />
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The{" "}
        <Typography.Link
          href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
        >
          GraphQL endpoint
        </Typography.Link>{" "}
        provides all information about components.
      </Typography.Paragraph>
    </Layout>
  );
}

export default Page;
