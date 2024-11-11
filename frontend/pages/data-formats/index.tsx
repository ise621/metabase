import { messageApolloError } from "../../lib/apollo";
import Layout from "../../components/Layout";
import { Typography } from "antd";
import { useDataFormatsQuery } from "../../queries/dataFormats.graphql";
import { useEffect } from "react";
import Link from "next/link";
import paths from "../../paths";
import { DataFormatTable } from "../../components/dataFormats/DataFormatTable";

// TODO Pagination. See https://www.apollographql.com/docs/react/pagination/core-api/

function Page() {
  const { loading, error, data } = useDataFormatsQuery();

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  return (
    <Layout>
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        <Link href={paths.data}>Data</Link> is shared as resources. Each
        resource has one of the following data formats:
      </Typography.Paragraph>
      <DataFormatTable
        loading={loading}
        dataFormats={data?.dataFormats?.edges?.map((e) => e.node) || []}
      />
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The{" "}
        <Typography.Link
          href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
        >
          GraphQL endpoint
        </Typography.Link>{" "}
        provides all information about data formats.
      </Typography.Paragraph>
    </Layout>
  );
}

export default Page;
