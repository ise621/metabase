import { message, Popconfirm, Skeleton, Space, Table, TableProps } from "antd";
import { Token } from "graphql";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import { UserRole } from "../../__generated__/__types__";
import { useEffect } from "react";
import { messageApolloError } from "../../lib/apollo";
import Link from "next/link";
import { ApplicationProps } from "../applications/Application";
import { useTokensByApplicationIdQuery } from "../../queries/token.graphql";

export default function TokenTable({ applicationId }: ApplicationProps) {
    const { loading, error, data } = useTokensByApplicationIdQuery({
      variables: {
        applicationId: applicationId,
      },
    });
    const tokens = data?.tokensByApplicationId;
    const currentUser = useCurrentUserQuery()?.data?.currentUser;

    function isAdmin() {
      return currentUser?.roles?.includes(UserRole.Administrator)
    }

    useEffect(() => {
      if (error) {
        messageApolloError(error);
      }
    }, [error]);

    if (loading) {
        return <Skeleton active avatar title />;
    }

    const tokenColumns: TableProps<Token>['columns'] = [
        {
            title: "Satus",
            dataIndex: "status",
            key: "status",
        },
        {
            title: "Type",
            dataIndex: "type",
            key: "type",
        },
        {
            title: "Subject",
            dataIndex: "subject",
            key: "subject",
        },
        {
            title: "Expiration Date",
            dataIndex: "expirationDate",
            key: "expirationDate",
        },
        {
            title: 'Action',
            key: 'action',
            render: (_, token) => (
                <Space size="middle">
                    {isAdmin() ? (
                        <>
                            {/* <Link href={paths.application(application.id!)}>Edit</Link> */}
                            <Popconfirm
                                title="Are you sure to revoke this token?"
                                onConfirm={() => {
                                    message.success('Revoke Token with id ' + token.id);
                                }}
                                okText="Yes"
                                cancelText="No"
                            >
                                <Link href="#">Revoke</Link>
                            </Popconfirm>
                        </>
                    )
                        : <></>}

                </Space>
            ),
        },
    ];


    return <>
        <Table<Token>
            loading={loading}
            columns={tokenColumns}
            dataSource={tokens as Token[]}
        />
    </>;
}