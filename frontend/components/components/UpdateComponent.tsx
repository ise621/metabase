import {
  ComponentsDocument,
  ComponentDocument,
  useUpdateComponentMutation,
} from "../../queries/components.graphql";
import dayjs from "dayjs";
import { Alert, Form, Input, Button, Modal, DatePicker, Select, Divider } from "antd";
import { useState } from "react";
import { handleFormErrors } from "../../lib/form";
import {
  ComponentCategory,
  DescriptionOrReference,
  DescriptionOrReferenceInput,
  OpenEndedDateTimeRange,
  Scalars,
} from "../../__generated__/__types__";
import { ReferenceForm } from "../ReferenceForm";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

type FormValues = {
  newName: string;
  newAbbreviation: string | null | undefined;
  newDescription: string;
  newAvailability:
    | [dayjs.Dayjs | null | undefined, dayjs.Dayjs | null | undefined]
    | null
    | undefined;
  newCategories: ComponentCategory[] | null | undefined;
  newPrimeSurface: DescriptionOrReferenceInput | null | undefined;
  newPrimeDirection: DescriptionOrReferenceInput | null | undefined;
  newSwitchableLayers: DescriptionOrReferenceInput | null | undefined;
};

export type UpdateComponentProps = {
  componentId: Scalars["Uuid"];
  name: string;
  abbreviation: string | null | undefined;
  description: string;
  availability: OpenEndedDateTimeRange | null | undefined;
  categories: ComponentCategory[] | null | undefined;
  primeSurface: DescriptionOrReference | null | undefined;
  primeDirection: DescriptionOrReference | null | undefined;
  switchableLayers: DescriptionOrReference | null | undefined;
};

export default function UpdateComponent({
  componentId,
  name,
  abbreviation,
  description,
  availability,
  categories,
  primeSurface,
  primeDirection,
  switchableLayers,
}: UpdateComponentProps) {
  const [open, setOpen] = useState(false);
  const [updateComponentMutation] = useUpdateComponentMutation({
    // TODO Update the cache more efficiently as explained on https://www.apollographql.com/docs/react/caching/cache-interaction/ and https://www.apollographql.com/docs/react/data/mutations/#making-all-other-cache-updates
    // See https://www.apollographql.com/docs/react/data/mutations/#options
    refetchQueries: [
      {
        query: ComponentsDocument,
      },
      {
        query: ComponentDocument,
        variables: {
          uuid: componentId,
        },
      },
    ],
  });
  const [globalErrorMessages, setGlobalErrorMessages] = useState(
    new Array<string>()
  );
  const [form] = Form.useForm<FormValues>();
  const [updating, setUpdating] = useState(false);

  const onFinish = ({
    newName,
    newAbbreviation,
    newDescription,
    newAvailability,
    newCategories,
    newPrimeSurface,
    newPrimeDirection,
    newSwitchableLayers,
  }: FormValues) => {
    const update = async () => {
      try {
        setUpdating(true);
        // TODO Why does `initialValue` not set standardizers to `[]`?
        if (newPrimeSurface?.reference?.standard != null && newPrimeSurface.reference.standard.standardizers == undefined) {
          newPrimeSurface.reference.standard.standardizers = [];
        }
        if (newPrimeDirection?.reference?.standard != null && newPrimeDirection.reference.standard.standardizers == undefined) {
          newPrimeDirection.reference.standard.standardizers = [];
        }
        if (newSwitchableLayers?.reference?.standard != null && newSwitchableLayers.reference.standard.standardizers == undefined) {
          newSwitchableLayers.reference.standard.standardizers = [];
        }
        // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout
        const { errors, data } = await updateComponentMutation({
          variables: {
            componentId: componentId,
            name: newName,
            abbreviation: newAbbreviation,
            description: newDescription,
            availability: {
              from: newAvailability?.[0],
              to: newAvailability?.[1],
            },
            categories: newCategories || [],
            primeSurface: newPrimeSurface,
            primeDirection: newPrimeDirection,
            switchableLayers: newSwitchableLayers,
          },
        });
        handleFormErrors(
          errors,
          data?.updateComponent?.errors?.map((x) => {
            return { code: x.code, message: x.message, path: x.path };
          }),
          setGlobalErrorMessages,
          form
        );
        if (
          !errors &&
          !data?.updateComponent?.errors &&
          data?.updateComponent?.component
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
      <Button onClick={() => setOpen(true)}>Edit</Button>
      <Modal
        open={open}
        title="Edit Component"
        // onOk={handleOk}
        onCancel={() => setOpen(false)}
        footer={false}
      >
        {/* TODO Display error messages in a list? */}
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
            label="Name"
            name="newName"
            rules={[
              {
                required: true,
              },
            ]}
            initialValue={name}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Abbreviation"
            name="newAbbreviation"
            initialValue={abbreviation}
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
            initialValue={description}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Availability"
            name="newAvailability"
            initialValue={[
              availability?.from == null ? null : dayjs(availability.from),
              availability?.to == null ? null : dayjs(availability.to),
            ]}
          >
            <DatePicker.RangePicker allowEmpty={[true, true]} showTime />
          </Form.Item>
          <Form.Item
            label="Categories"
            name="newCategories"
            initialValue={categories}
          >
            <Select
              mode="multiple"
              placeholder="Please select"
              options={Object.entries(ComponentCategory).map(
                ([_key, value]) => ({
                  label: value,
                  value: value,
                })
              )}
            />
          </Form.Item>
          <Divider />
          <Form.Item label="Prime Surface" name="newPrimeSurface">
            <Form.Item
              label="Description"
              name={["newPrimeSurface", "description"]}
              initialValue={primeSurface?.description}
            >
              <Input />
            </Form.Item>
            <ReferenceForm form={form} namespace={["newPrimeSurface", "reference"]} initialValue={primeSurface?.reference} />
          </Form.Item>
          <Form.Item label="Prime Direction" name="newPrimeDirection">
            <Form.Item
              label="Description"
              name={["newPrimeDirection", "description"]}
              initialValue={primeDirection?.description}
            >
              <Input />
            </Form.Item>
            <ReferenceForm form={form} namespace={["newPrimeDirection", "reference"]} initialValue={primeDirection?.reference} />
          </Form.Item>
          <Form.Item label="Switchable Layers" name="newSwitchableLayers">
            <Form.Item
              label="Description"
              name={["newSwitchableLayers", "description"]}
              initialValue={switchableLayers?.description}
            >
              <Input />
            </Form.Item>
            <ReferenceForm form={form} namespace={["newSwitchableLayers", "reference"]} initialValue={switchableLayers?.reference} />
          </Form.Item>
          <Form.Item {...tailLayout}>
            <Button type="primary" htmlType="submit" loading={updating}>
              Update
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
