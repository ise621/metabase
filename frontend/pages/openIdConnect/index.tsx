import { messageApolloError } from "../../lib/apollo";
import Layout from "../../components/Layout";
import paths from "../../paths";
import { Table, Typography, TableProps, Space } from "antd";
import { useOpenIdConnectQuery } from "../../queries/openIdConnect.graphql";
import { useEffect } from "react";
import { OpenIdConnectApplication, UserRole } from "../../__generated__/__types__";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import Link from "next/link";

// TODO Load and display scopes.

function Page() {
  const { loading, error, data } = useOpenIdConnectQuery();
  const currentUser = useCurrentUserQuery()?.data?.currentUser;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  function isAdmin() {
    return currentUser?.roles?.includes(UserRole.Administrator)
  }

  const applicationColumns: TableProps<OpenIdConnectApplication>['columns'] = [
    {
      title: "Name",
      dataIndex: "displayName",
      key: "displayName",
    },
    {
      title: 'Action',
      key: 'action',
      render: (_, application) => (
        <Space size="middle">
          {isAdmin() ? (
            <>
              <Link href={paths.openIdApplication(application.id!)}>Edit</Link>
              <Link href={paths.calorimetricData}>Delete</Link>
            </>
          )
            : <></>}

        </Space>
      ),
    },
  ];

  return (
    <Layout>
      <Typography.Title>Applications</Typography.Title>
      <Table<OpenIdConnectApplication>
        loading={loading}
        columns={applicationColumns}
        dataSource={data?.openIdConnectApplications as Array<OpenIdConnectApplication> || []}
      />
      <Typography.Title>Authorizations</Typography.Title>
      <Table
        loading={loading}
        columns={[
          {
            title: "concurrencyToken",
            dataIndex: "concurrencyToken",
            key: "concurrencyToken",
          },
          {
            title: "creationDate",
            dataIndex: "creationDate",
            key: "creationDate",
          },
          {
            title: "id",
            dataIndex: "id",
            key: "id",
          },
          {
            title: "properties",
            dataIndex: "properties",
            key: "properties",
          },
          {
            title: "scopes",
            dataIndex: "scopes",
            key: "scopes",
          },
          {
            title: "status",
            dataIndex: "status",
            key: "status",
          },
          {
            title: "subject",
            dataIndex: "subject",
            key: "subject",
          },
          {
            title: "type",
            dataIndex: "type",
            key: "type",
          },
          //   {
          //     title: "tokens",
          //     dataIndex: "tokens",
          //     key: "tokens",
          //     render: (_value, record, _index) => (
          //     ),
          //   },
        ]}
        dataSource={data?.openIdConnectAuthorizations || []}
      />
      <Typography.Title>Tokens</Typography.Title>
      <Table
        loading={loading}
        columns={[
          {
            title: "concurrencyToken",
            dataIndex: "concurrencyToken",
            key: "concurrencyToken",
          },
          {
            title: "creationDate",
            dataIndex: "creationDate",
            key: "creationDate",
          },
          {
            title: "expirationDate",
            dataIndex: "expirationDate",
            key: "expirationDate",
          },
          {
            title: "id",
            dataIndex: "id",
            key: "id",
          },
          {
            title: "payload",
            dataIndex: "payload",
            key: "payload",
          },
          {
            title: "properties",
            dataIndex: "properties",
            key: "properties",
          },
          {
            title: "redemptionDate",
            dataIndex: "redemptionDate",
            key: "redemptionDate",
          },
          {
            title: "referenceId",
            dataIndex: "referenceId",
            key: "referenceId",
          },
          {
            title: "status",
            dataIndex: "status",
            key: "status",
          },
          {
            title: "subject",
            dataIndex: "subject",
            key: "subject",
          },
          {
            title: "type",
            dataIndex: "type",
            key: "type",
          },
        ]}
        dataSource={data?.openIdConnectTokens || []}
      />
      <Typography.Title>Scopes</Typography.Title>
      <Table
        loading={loading}
        columns={[
          {
            title: "concurrencyToken",
            dataIndex: "concurrencyToken",
            key: "concurrencyToken",
          },
          {
            title: "description",
            dataIndex: "description",
            key: "description",
          },
          {
            title: "descriptions",
            dataIndex: "descriptions",
            key: "descriptions",
          },
          {
            title: "displayName",
            dataIndex: "displayName",
            key: "displayName",
          },
          {
            title: "displayNames",
            dataIndex: "displayNames",
            key: "displayNames",
          },
          {
            title: "id",
            dataIndex: "id",
            key: "id",
          },
          {
            title: "name",
            dataIndex: "name",
            key: "name",
          },
          {
            title: "properties",
            dataIndex: "properties",
            key: "properties",
          },
          {
            title: "resources",
            dataIndex: "resources",
            key: "resources",
          },
        ]}
        dataSource={data?.openIdConnectScopes || []}
      />
    </Layout>
  );
}

export default Page;
