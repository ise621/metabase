import { Application } from "../../__generated__/__types__";
import { Button, Flex, message, Popconfirm, Skeleton, Space, Table, TableProps, Typography } from "antd";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import Link from "next/link";
import paths from "../../paths";
import { useRouter } from "next/router";
import { ApplicationsDocument, useDeleteApplicationMutation } from "../../queries/applications.graphql";

export type ApplicationsProps = {
    editable: boolean | undefined;
    loading: boolean;
    applications: Array<Application>;
};

export default function ApplicationTable({ loading, applications }: ApplicationsProps) {
    const router = useRouter();
    const currentUser = useCurrentUserQuery()?.data?.currentUser;
    const [deleteApplicationMutation] = useDeleteApplicationMutation({
        // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
        // See https://www.apollographql.com/docs/react/data/mutations/#options
        refetchQueries: [
            {
                query: ApplicationsDocument,
            },
        ],
    });

    if (loading) {
        return <Skeleton active avatar title />;
    }

    const applicationColumns: TableProps<Application>['columns'] = [
        {
            title: "Name",
            dataIndex: "displayName",
            key: "displayName",
        },
        {
            title: "Client Id",
            dataIndex: "clientId",
            key: "clientId",
        },
        {
            title: "Redirect URL",
            dataIndex: "redirectUri",
            key: "redirectUri",
        },
        {
            title: "Logout Redirect",
            dataIndex: "postLogoutRedirectUri",
            key: "postLogoutRedirectUri",
        },
        {
            title: 'Action',
            key: 'action',
            render: (_, application) => (
                <Space key={`space_${application.id}`} size="middle">
                    {application.canCurrentUserManageApplication ? (
                        <>
                            <Link key={`edit_${application.id}`} href={paths.application(application.id!)}>Edit</Link>
                            <Popconfirm key={`popcon_${application.id}`}
                                title="Are you sure to delete this application?"
                                onConfirm={async () => {
                                    const { data } = await deleteApplicationMutation({
                                        variables: {
                                            applicationId: application.id!
                                        },
                                    });
                                    if (data?.deleteApplication.errors) {
                                        message.error(data?.deleteApplication.errors.toString())
                                    }
                                    else {
                                        message.success('Successfully deleted application ' + application.displayName)
                                    }
                                }}
                                okText="Yes"
                                cancelText="No"
                            >
                                <Link key={`delete_${application.id}`} href="#">Delete</Link>
                            </Popconfirm>
                        </>
                    )
                        : <></>}

                </Space>
            ),
        },
    ];


    return <>
        <Typography.Title>Applications</Typography.Title>
        {currentUser?.canCurrentUserAddApplications ? (
            <>
                <Flex justify="right" gap="small">
                    <Button type="primary" onClick={() => router.push(paths.applicationCreate)} style={{ marginBottom: "5px" }}>Add Application</Button>
                </Flex>
            </>
        )
            : <></>}
        <Table<Application>
            loading={loading}
            columns={applicationColumns}
            dataSource={applications}
        />
    </>;
}