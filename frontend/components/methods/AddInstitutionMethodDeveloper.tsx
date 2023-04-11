import * as React from "react";
import { Alert, Form, Button } from "antd";
import { Scalars } from "../../__generated__/__types__";
import { useState } from "react";
import { handleFormErrors } from "../../lib/form";
import { MethodDocument } from "../../queries/methods.graphql";
import { SelectInstitutionId } from "../SelectInstitutionId";
import { useAddInstitutionMethodDeveloperMutation } from "../../queries/institutionMethodDevelopers.graphql";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

type FormValues = { institutionId: Scalars["Uuid"] };

export type AddInstitutionMethodDeveloperProps = {
  methodId: Scalars["Uuid"];
};

export default function AddInstitutionMethodDeveloper({
  methodId,
}: AddInstitutionMethodDeveloperProps) {
  const [addInstitutionMethodDeveloperMutation] =
    useAddInstitutionMethodDeveloperMutation({
      // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
      // See https://www.apollographql.com/docs/react/data/mutations/#options
      refetchQueries: [
        {
          query: MethodDocument,
          variables: {
            uuid: methodId,
          },
        },
      ],
    });
  const [globalErrorMessages, setGlobalErrorMessages] = useState(
    new Array<string>()
  );
  const [form] = Form.useForm<FormValues>();
  const [adding, setAdding] = useState(false);

  const onFinish = ({ institutionId }: FormValues) => {
    const add = async () => {
      try {
        setAdding(true);
        // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout
        const { errors, data } = await addInstitutionMethodDeveloperMutation({
          variables: {
            methodId: methodId,
            institutionId: institutionId,
          },
        });
        handleFormErrors(
          errors,
          data?.addInstitutionMethodDeveloper?.errors?.map((x) => {
            return { code: x.code, message: x.message, path: x.path };
          }),
          setGlobalErrorMessages,
          form
        );
        if (!errors && !data?.addInstitutionMethodDeveloper?.errors) {
          form.resetFields();
        }
      } catch (error) {
        // TODO Handle properly.
        console.log("Failed:", error);
      } finally {
        setAdding(false);
      }
    };
    add();
  };

  const onFinishFailed = () => {
    setGlobalErrorMessages(["Fix the errors below."]);
  };

  return (
    <>
      {globalErrorMessages.length > 0 ? (
        <Alert type="error" message={globalErrorMessages.join(" ")} />
      ) : (
        <></>
      )}
      <Form
        {...layout}
        form={form}
        name="addInstitutionMethodDeveloper"
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
      >
        <Form.Item
          label="Institution"
          name="institutionId"
          rules={[
            {
              required: true,
            },
          ]}
        >
          <SelectInstitutionId />
        </Form.Item>
        <Form.Item {...tailLayout}>
          <Button type="primary" htmlType="submit" loading={adding}>
            Add
          </Button>
        </Form.Item>
      </Form>
    </>
  );
}
