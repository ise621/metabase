import { useEffect, useState } from "react";
import { ApplicationsDocument, useCreateApplicationMutation } from "../../queries/applications.graphql";
import { Alert, Button, Col, Flex, Form, Input, message, Row, Select } from "antd";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import { useRouter } from "next/router";
import { handleFormErrors } from "../../lib/form";
import paths, { redirectToLoginPage } from "../../paths";
import { useScopesQuery } from "../../queries/scopes.graphql";

type FormValues = {
    clientId: string;
    displayName: string;
    redirectUri: string;
    postLogoutRedirectUri: string;
    permissions: string[];
};

export default function CreateApplication() {
    const scopes = useScopesQuery()?.data?.scopes;
    // const institutions = useIn
    const currentUser = useCurrentUserQuery()?.data?.currentUser;
    const router = useRouter();
    const shouldRedirect = !(currentUser);
    const [form] = Form.useForm<FormValues>();
    const [updating, setUpdating] = useState(false);
    const [globalErrorMessages, setGlobalErrorMessages] = useState(new Array<string>());

    const [createApplicationMutation] = useCreateApplicationMutation({
        // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
        // See https://www.apollographql.com/docs/react/data/mutations/#options
        refetchQueries: [
            {
                query: ApplicationsDocument,
            },
        ],
    });

    const onFinish = ({
        clientId,
        displayName,
        redirectUri,
        postLogoutRedirectUri,
        permissions,
    }: FormValues) => {
        const update = async () => {
            try {
                setUpdating(true);
                // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout

                const { errors, data } = await createApplicationMutation({
                    variables: {
                        clientId: clientId,
                        displayName: displayName,
                        redirectUri: redirectUri,
                        postLogoutRedirectUri: postLogoutRedirectUri,
                        permissions: JSON.stringify(permissions),
                    },
                });
                handleFormErrors(
                    errors,
                    data?.createApplication?.errors?.map((x) => {
                        return { code: x.code, message: x.message, path: x.path };
                    }),
                    setGlobalErrorMessages,
                    form
                );
                if (data) {
                    message.success('Successfully created application ' + data.createApplication.application?.displayName)
                    console.log('Client Secret: ' + data.createApplication.application?.clientSecret)
                    router.push(paths.openIdConnect)
                }
            } catch (error) {
                // TODO Handle properly.
                message.error("Failed:" + error);
            } finally {
                setUpdating(false);
            }
        };
        update();
    };

    const onFinishFailed = () => {
        setGlobalErrorMessages(["Fix the errors below."]);
    };

    useEffect(() => {
        if (router.isReady && shouldRedirect) {
            redirectToLoginPage(router, paths.applicationCreate);
        }
    }, [shouldRedirect, router]);

    return (
        <>
            {globalErrorMessages.length > 0 ? (
                <Alert className="error-message" type="error" message={globalErrorMessages.join(" ")} />
            ) : (
                <></>
            )}
            <Form
                labelAlign="left"
                labelCol={{ flex: "150px" }}
                wrapperCol={{ flex: "auto" }}
                form={form}
                name="basic"
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
            >
                <Form.Item
                    label="Assosiated Institution"
                    name="assosiatedInstitution"
                    // rules={[{ required: true }]}
                >
                    <Select
                        allowClear
                        style={{ width: '100%' }}
                        placeholder="Please select"
                    >
                        {/* {scopes?.map((scope => {
                            return <Select.Option key={scope.id!} value={scope.name}>
                                {scope.displayName}
                            </Select.Option>
                        }))
                        } */}
                    </Select>
                </Form.Item>
                <Form.Item
                    label="Client Id"
                    name="clientId"
                    rules={[{ required: true }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    label="Display Name"
                    name="displayName"
                    rules={[{ required: true }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    label="Login Redirect URL"
                    name="redirectUri"
                    rules={[{ required: true }, { type: 'url' }, { type: 'string', min: 6 }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    label="Logout Redirect URL"
                    name="postLogoutRedirectUri"
                    rules={[{ required: true }, { type: 'url' }, { type: 'string', min: 6 }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    label="Permissions"
                    name="permissions"
                    rules={[{ required: true }]}
                >
                    <Select
                        mode="multiple"
                        allowClear
                        style={{ width: '100%' }}
                        placeholder="Please select"
                    >
                        {scopes?.map((scope => {
                            return <Select.Option key={scope.id!} value={scope.name}>
                                {scope.displayName}
                            </Select.Option>
                        }))
                        }
                    </Select>
                </Form.Item>
                <Form.Item>
                    <Flex gap="small" justify="right">
                        <Button type="primary"
                            htmlType="button"
                            loading={updating}
                            href={paths.openIdConnect}>
                            Cancel
                        </Button>
                        <Button type="primary"
                            htmlType="submit"
                            loading={updating}
                            style={{ marginLeft: "5px" }}>
                            Create
                        </Button>
                    </Flex>
                </Form.Item>
            </Form>
        </>
    );
}