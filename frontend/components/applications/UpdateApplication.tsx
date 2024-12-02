import { useEffect, useState } from "react";
import { useUpdateApplicationMutation, ApplicationsDocument, useApplicationQuery, ApplicationPartialFragment } from "../../queries/applications.graphql";
import { Alert, Button, Flex, Form, Input, message, Select, Skeleton } from "antd";
import { messageApolloError } from "../../lib/apollo";
import { useRouter } from "next/router";
import { handleFormErrors } from "../../lib/form";
import { useScopesQuery } from "../../queries/scopes.graphql";
import paths from "../../paths";
import { ApplicationProps } from "./Application";

type FormValues = {
  newClientId: string;
  newDisplayName: string;
  newRedirectUri: string;
  newPostLogoutRedirectUri: string;
  newPermissions: string;
};

export default function UpdateApplication({ applicationId }: ApplicationProps) {
  const { loading, error, data } = useApplicationQuery({
    variables: {
      uuid: applicationId,
    },
  });
  const scopes = useScopesQuery()?.data?.scopes;
  const application = data?.application as ApplicationPartialFragment;
  const router = useRouter();
  const [form] = Form.useForm<FormValues>();
  const [updating, setUpdating] = useState(false);
  const [globalErrorMessages, setGlobalErrorMessages] = useState(new Array<string>());

  const [updateApplicationMutation] = useUpdateApplicationMutation({
    // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
    // See https://www.apollographql.com/docs/react/data/mutations/#options
    refetchQueries: [
      {
        query: ApplicationsDocument,
      },
    ],
  });

  const onFinish = ({
    newClientId,
    newDisplayName,
    newRedirectUri,
    newPostLogoutRedirectUri,
    newPermissions,
  }: FormValues) => {
    const update = async () => {
      try {
        setUpdating(true);
        const { errors, data } = await updateApplicationMutation({
          variables: {
            id: applicationId,
            clientId: newClientId,
            displayName: newDisplayName,
            redirectUri: newRedirectUri,
            postLogoutRedirectUri: newPostLogoutRedirectUri,
            permissions: JSON.stringify(newPermissions),
          },
        });
        handleFormErrors(
          errors,
          data?.updateApplication?.errors?.map((x) => {
            return { code: x.code, message: x.message, path: x.path };
          }),
          setGlobalErrorMessages,
          form
        );
        if (data) {
          message.success('Successfully updated application ' + data.updateApplication.application?.displayName)
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
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

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
          label="ClientId"
          name="newClientId"
          rules={[{ required: true }]}
          initialValue={application ? application.clientId : ""}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Display Name"
          name="newDisplayName"
          rules={[{ required: true }]}
          initialValue={application ? application.displayName : ""}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Login Redirect URL"
          name="newRedirectUri"
          rules={[{ required: true }, { type: 'url' }, { type: 'string', min: 6 }]}
          initialValue={application ? application.redirectUris : ""}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Logout Redirect URL"
          name="newPostLogoutRedirectUri"
          rules={[{ required: true }, { type: 'url' }, { type: 'string', min: 6 }]}
          initialValue={application ? application.postLogoutRedirectUris : ""}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Permissions"
          name="newPermissions"
          rules={[{ required: true }]}
          initialValue={application?.permissions}
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
                Update
              </Button>
            </Flex>          
        </Form.Item>
      </Form>
    </>
  );
}