import { message, Popconfirm, Skeleton, Space, Table, TableProps } from "antd";
import { Token } from "../../__generated__/__types__";
import { useEffect } from "react";
import { messageApolloError } from "../../lib/apollo";
import Link from "next/link";
import { ApplicationProps } from "../applications/Application";
import { TokensByApplicationIdDocument, useRevokeTokenMutation, useTokensByApplicationIdQuery } from "../../queries/token.graphql";

export default function TokenTable({ applicationId }: ApplicationProps) {
    const { loading, error, data } = useTokensByApplicationIdQuery({
      variables: {
        applicationId: applicationId,
      },
    });
    const tokens = data?.tokensByApplicationId;
    const [revokeTokenMutation] = useRevokeTokenMutation({
        // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
        // See https://www.apollographql.com/docs/react/data/mutations/#options
        refetchQueries: [
            {
                query: TokensByApplicationIdDocument,
            },
        ],
    });

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
                    {token.canCurrentUserRevokeToken ? (
                        <>
                            <Popconfirm
                                title="Are you sure to revoke this token?"
                                onConfirm={async () => {
                                    const { data } = await revokeTokenMutation({
                                        variables: {
                                            tokenId: token.id!
                                        },
                                    });
                                    if (data?.revokeToken.errors) {
                                        message.error(data?.revokeToken.errors.toString())
                                    }
                                    else {
                                        message.success('Successfully revoked token ' + token.id)
                                    }
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