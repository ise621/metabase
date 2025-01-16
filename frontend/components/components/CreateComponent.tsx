import * as React from "react";
import { DatePicker, Alert, Select, Form, Input, Button, Divider } from "antd";
import {
  useCreateComponentMutation,
  ComponentsDocument,
} from "../../queries/components.graphql";
import { ComponentCategory, DescriptionOrReferenceInput, Scalars } from "../../__generated__/__types__";
import { useState } from "react";
import { handleFormErrors } from "../../lib/form";
import dayjs from "dayjs";
import { InstitutionDocument } from "../../queries/institutions.graphql";
import { ReferenceForm } from "../ReferenceForm";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

type FormValues = {
  name: string;
  abbreviation: string | null | undefined;
  description: string;
  availability:
    | [dayjs.Dayjs | null | undefined, dayjs.Dayjs | null | undefined]
    | null
    | undefined;
  categories: ComponentCategory[] | null | undefined;
  primeSurface: DescriptionOrReferenceInput | null | undefined;
  primeDirection: DescriptionOrReferenceInput | null | undefined;
  switchableLayers: DescriptionOrReferenceInput | null | undefined;
};

export type CreateComponentProps = {
  manufacturerId: Scalars["Uuid"];
};

export default function CreateComponent({
  manufacturerId,
}: CreateComponentProps) {
  const [createComponentMutation] = useCreateComponentMutation({
    // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
    // See https://www.apollographql.com/docs/react/data/mutations/#options
    refetchQueries: [
      {
        query: InstitutionDocument,
        variables: {
          uuid: manufacturerId,
        },
      },
      {
        query: ComponentsDocument,
      },
    ],
  });
  const [globalErrorMessages, setGlobalErrorMessages] = useState(
    new Array<string>()
  );
  const [form] = Form.useForm<FormValues>();
  const [creating, setCreating] = useState(false);

  const onFinish = ({
    name,
    abbreviation,
    description,
    availability,
    categories,
    primeSurface,
    primeDirection,
    switchableLayers,
  }: FormValues) => {
    const create = async () => {
      try {
        setCreating(true);
        // TODO Why does `initialValue` not set standardizers to `[]`?
        if (primeSurface?.reference?.standard != null && primeSurface.reference.standard.standardizers == undefined) {
          primeSurface.reference.standard.standardizers = [];
        }
        if (primeDirection?.reference?.standard != null && primeDirection.reference.standard.standardizers == undefined) {
          primeDirection.reference.standard.standardizers = [];
        }
        if (switchableLayers?.reference?.standard != null && switchableLayers.reference.standard.standardizers == undefined) {
          switchableLayers.reference.standard.standardizers = [];
        }
        // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout
        const { errors, data } = await createComponentMutation({
          variables: {
            name: name,
            abbreviation: abbreviation,
            description: description,
            availability: { from: availability?.[0], to: availability?.[1] },
            categories: categories || [],
            primeSurface: primeSurface,
            primeDirection: primeDirection,
            switchableLayers: switchableLayers,
            manufacturerId: manufacturerId,
          },
        });
        handleFormErrors(
          errors,
          data?.createComponent?.errors?.map((x) => {
            return { code: x.code, message: x.message, path: x.path };
          }),
          setGlobalErrorMessages,
          form
        );
        if (!errors && !data?.createComponent?.errors) {
          form.resetFields();
        }
      } catch (error) {
        // TODO Handle properly.
        console.log("Failed:", error);
      } finally {
        setCreating(false);
      }
    };
    create();
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
        name="createComponent"
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
      >
        <Form.Item
          label="Name"
          name="name"
          rules={[
            {
              required: true,
            },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item label="Abbreviation" name="abbreviation">
          <Input />
        </Form.Item>
        <Form.Item
          label="Description"
          name="description"
          rules={[
            {
              required: true,
            },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item label="Availability" name="availability">
          <DatePicker.RangePicker allowEmpty={[true, true]} showTime />
        </Form.Item>
        <Form.Item label="Categories" name="categories">
          <Select
            mode="multiple"
            placeholder="Please select"
            options={Object.entries(ComponentCategory).map(([_key, value]) => ({
              label: value,
              value: value,
            }))}
          />
        </Form.Item>
        <Divider />
        <Form.Item label="Prime Surface" name="primeSurface">
          <Form.Item
            label="Description"
            name={["primeSurface", "description"]}
          >
            <Input />
          </Form.Item>
          <ReferenceForm form={form} namespace={["primeSurface", "reference"]} />
        </Form.Item>
        <Form.Item label="Prime Direction" name="primeDirection">
          <Form.Item
            label="Description"
            name={["primeDirection", "description"]}
          >
            <Input />
          </Form.Item>
          <ReferenceForm form={form} namespace={["primeDirection", "reference"]} />
        </Form.Item>
        <Form.Item label="Switchable Layers" name="switchableLayers">
          <Form.Item
            label="Description"
            name={["switchableLayers", "description"]}
          >
            <Input />
          </Form.Item>
          <ReferenceForm form={form} namespace={["switchableLayers", "reference"]} />
        </Form.Item>
        <Form.Item {...tailLayout}>
          <Button type="primary" htmlType="submit" loading={creating}>
            Create
          </Button>
        </Form.Item>
      </Form>
    </>
  );
}
