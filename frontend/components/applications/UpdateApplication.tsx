import { useEffect, useState } from "react";
import { Scalars } from "../../__generated__/__types__";
import { useApplicationQuery, useUpdateApplicationMutation, ApplicationsDocument } from "../../queries/application.graphql";
import { messageApolloError } from "../../lib/apollo";
import { Alert, Button, Form, Input, Modal, Result, Skeleton } from "antd";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};

const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

export type ApplicationProps = {
  applicationId: Scalars["Uuid"];
};

type FormValues = {
  newClientId: string;
  newClientSecret: string;
  newDisplayName: string;
  newPermissions: string;
};
  
export default function UpdateApplication({ applicationId }: ApplicationProps) {
  const { loading, error, data } = useApplicationQuery({
    variables: {
      uuid: applicationId,
    },
  });
  var application = data?.openIdConnectApplication
  
  const [updateApplicationMutation] = useUpdateApplicationMutation({
    // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
    // See https://www.apollographql.com/docs/react/data/mutations/#options
    refetchQueries: [
      {
        query: ApplicationsDocument,
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

  if (!application) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }
  
  const [globalErrorMessages, setGlobalErrorMessages] = useState(
    new Array<string>()
  );
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm<FormValues>();
  const [updating, setUpdating] = useState(false);

  const onFinish = ({
    newClientId,
    newClientSecret,
    newDisplayName,
    newPermissions,
  }: FormValues) => {
    const update = async () => {
      try {
        setUpdating(true);
        // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout
        const { data } = await updateApplicationMutation({
          variables: {
            id: applicationId,
            clientId: newClientId,
            clientSecret: newClientSecret,
            displayName: newDisplayName,
            permissions: newPermissions,
          },
        });
        // handleFormErrors(
        //   errors,
        //   data?.updateInstitution?.errors?.map((x) => {
        //     return { code: x.code, message: x.message, path: x.path };
        //   }),
        //   setGlobalErrorMessages,
        //   form
        // );
        if (
          data?.updateApplication
        ) {
          setOpen(false);
        }
      } catch (error) {
        // TODO Handle properly.
        console.log("Failed:", error);
      } finally {
        setUpdating(false);
      }
    };
    update();
  };

  const onFinishFailed = () => {
    setGlobalErrorMessages(["Fix the errors below."]);
  };

  return (
    <>
      {/* <Button onClick={() => setOpen(true)}>Edit</Button>
      <Modal
        open={open}
        title="Edit Application"
        // onOk={handleOk}
        onCancel={() => setOpen(false)}
        footer={false}
      >
        {}
        {globalErrorMessages.length > 0 ? (
          <Alert type="error" message={globalErrorMessages.join(" ")} />
        ) : (
          <></>
        )}
        <Form
          {...layout}
          form={form}
          name="basic"
          onFinish={onFinish}
          onFinishFailed={onFinishFailed}
        >
          <Form.Item
            label="ClientId"
            name="newClientId"
            rules={[
              {
                required: true,
              },
            ]}
            initialValue={application.clientId}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Client Secret"
            name="newClientSecret"
            initialValue={application.clientSecret}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Description"
            name="newDescription"
            rules={[
              {
                required: true,
              },
            ]}
            initialValue={application.displayName}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Permissions"
            name="newPermissons"
            rules={[
              {
                required: true,
              },
            ]}
            initialValue={application.permissions}
          >
            <Input />
          </Form.Item>
          <Form.Item {...tailLayout}>
            <Button type="primary" htmlType="submit" loading={updating}>
              Update
            </Button>
          </Form.Item>
        </Form>
      </Modal> */}
    </>
  );
}