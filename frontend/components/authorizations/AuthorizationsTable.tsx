import { Authorization, UserRole } from "../../__generated__/__types__";
import { message, Popconfirm, Skeleton, Space, Table, TableProps } from "antd";
import Link from "next/link";
import { ApplicationProps } from "../applications/Application";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import { useEffect } from "react";
import { messageApolloError } from "../../lib/apollo";
import { useAuthorizationsByApplicationIdQuery } from "../../queries/authorizations.graphql";

export default function AutorizationsTable({ applicationId }: ApplicationProps) {
    const { loading, error, data } = useAuthorizationsByApplicationIdQuery({
      variables: {
        applicationId: applicationId,
      },
    });
    const authorizations = data?.authorizationsByApplicationId;
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

    const authorizationColumns: TableProps<Authorization>['columns'] = [
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
            title: 'Action',
            key: 'action',
            render: (_, authentication) => (
                <Space size="middle">
                    {isAdmin() ? (
                        <>
                            {/* <Link href={paths.application(application.id!)}>Edit</Link> */}
                            <Popconfirm
                                title="Are you sure to delete this authorization?"
                                onConfirm={() => {
                                    message.success('Delete ' + authentication.id);
                                }}
                                okText="Yes"
                                cancelText="No"
                            >
                                <Link href="#">Delete</Link>
                            </Popconfirm>
                        </>
                    )
                        : <></>}

                </Space>
            ),
        },
    ];


    return <>
        <Table<Authorization>
            loading={loading}
            columns={authorizationColumns}
            dataSource={authorizations as Authorization[]}
        />
    </>;
}