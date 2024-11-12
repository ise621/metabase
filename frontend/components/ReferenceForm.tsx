import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";
import { InputNumber, Select, Form, Input, Button, FormInstance } from "antd";
import { useState } from "react";
import {
  Standardizer,
  Standard,
  Publication,
} from "../__generated__/__types__";

const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

enum ReferenceKind {
  None = "None",
  Standard = "Standard",
  Publication = "Publication",
}

function referenceToKind(
  reference: Publication | Standard | null | undefined
): ReferenceKind {
  switch (reference?.__typename) {
    case null:
      return ReferenceKind.None;
    case "Standard":
      return ReferenceKind.Standard;
    case "Publication":
      return ReferenceKind.Publication;
    default:
      // TODO Why does this not work? For a working example see https://www.typescriptlang.org/docs/handbook/2/narrowing.html#exhaustiveness-checking
      // const _exhaustiveCheck: never = reference;
      // return _exhaustiveCheck;
      return ReferenceKind.None;
  }
}

// interface HasStandardAndPublication {
//   standard: CreateStandardInput | null | undefined;
//   publication: CreatePublicationInput | null | undefined;
// }

export type ReferenceFormProps<Values> = {
  form: FormInstance<Values>;
  initialValue?: Standard | Publication | null;
  namespace?: string[];
};

// TODO Why does the following not work? export function ReferenceForm<Values extends HasStandardAndPublication>({form}: ReferenceFormProps<Values>) {
export function ReferenceForm({ form, initialValue, namespace = [] }: ReferenceFormProps<any>) {
  const initialKind = referenceToKind(initialValue);
  const [selectedReferenceOption, setSelectedReferenceOption] =
    useState(initialKind);
  if (initialValue != null) {
    const { __typename, ...initialReference } = initialValue
    if (__typename == "Publication") {
      form.setFieldValue(namespace.concat("reference", "publication"), initialReference);
    }
    if (__typename == "Standard") {
      form.setFieldValue(namespace.concat("reference", "standard"), initialReference);
    }
  }

  const onReferenceChange = (value: ReferenceKind) => {
    switch (value) {
      case ReferenceKind.None:
        form.setFieldsValue({ standard: null });
        form.setFieldsValue({ publication: null });
        break;
      case ReferenceKind.Publication:
        form.setFieldsValue({ standard: null });
        if (initialValue != null) {
          const { __typename, ...initialReference } = initialValue
          if (__typename == "Publication") {
            form.setFieldValue(namespace.concat("reference", "publication"), initialReference);
          }
        }
        break;
      case ReferenceKind.Standard:
        form.setFieldsValue({ publication: null });
        if (initialValue != null) {
          const { __typename, ...initialReference } = initialValue
          if (__typename == "Standard") {
            form.setFieldValue(namespace.concat("reference", "standard"), initialReference);
          }
        }
        break;
      default:
        console.error("Impossible!");
    }
    setSelectedReferenceOption(value);
  };

  return (
    <>
      <Form.Item label="Reference" name="unmappedReferenceKind" initialValue={initialKind}>
        <Select
          options={[
            { label: "None", value: ReferenceKind.None },
            { label: "Standard", value: ReferenceKind.Standard },
            { label: "Publication", value: ReferenceKind.Publication },
          ]}
          onChange={onReferenceChange}
        />
      </Form.Item>
      {selectedReferenceOption === ReferenceKind.Publication && (
        <>
          <Form.Item label="Title" name={namespace.concat("reference", "publication", "title")}>
            <Input />
          </Form.Item>
          <Form.Item label="Abstract" name={namespace.concat("reference", "publication", "abstract")}>
            <Input />
          </Form.Item>
          <Form.Item label="Section" name={namespace.concat("reference", "publication", "section")}>
            <Input />
          </Form.Item>
          <Form.Item label="arXiv" name={namespace.concat("reference", "publication", "arXiv")}>
            <Input />
          </Form.Item>
          <Form.Item label="DOI" name={namespace.concat("reference", "publication", "doi")}>
            <Input />
          </Form.Item>
          <Form.Item label="URN" name={namespace.concat("reference", "publication", "urn")}>
            <Input />
          </Form.Item>
          <Form.Item
            label="WebAddress"
            name={namespace.concat("reference", "publication", "webAddress")}
            rules={[
              {
                type: "url",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.List name={namespace.concat("reference", "publication", "authors")}>
            {(fields, { add, remove }, { errors }) => (
              <>
                {fields.map((field, index) => (
                  <Form.Item
                    key={field.key}
                    label={index === 0 ? "Authors" : " "}
                  >
                    <Input.Group>
                      <Form.Item {...field} noStyle>
                        <Input style={{ width: "90%" }} />
                      </Form.Item>
                      <MinusCircleOutlined
                        style={{ width: "10%" }}
                        onClick={() => remove(field.name)}
                      />
                    </Input.Group>
                  </Form.Item>
                ))}
                <Form.Item {...tailLayout}>
                  <Button
                    type="dashed"
                    onClick={() => add()}
                    style={{ width: "100%" }}
                    icon={<PlusOutlined />}
                  >
                    Add author
                  </Button>
                  <Form.ErrorList errors={errors} />
                </Form.Item>
              </>
            )}
          </Form.List>
        </>
      )}
      {selectedReferenceOption === ReferenceKind.Standard && (
        <>
          <Form.Item label="Title" name={namespace.concat("reference", "standard", "title")}>
            <Input />
          </Form.Item>
          <Form.Item label="Abstract" name={namespace.concat("reference", "standard", "abstract")}>
            <Input />
          </Form.Item>
          <Form.Item label="Section" name={namespace.concat("reference", "standard", "section")}>
            <Input />
          </Form.Item>
          <Form.Item label="Numeration">
            <Input.Group>
              <Form.Item
                noStyle
                name={namespace.concat("reference", "standard", "numeration", "mainNumber")}
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Input placeholder="Main Number" />
              </Form.Item>
              <Form.Item noStyle name={namespace.concat("reference", "standard", "numeration", "prefix")}>
                <Input placeholder="Prefix" />
              </Form.Item>
              <Form.Item noStyle name={namespace.concat("reference", "standard", "numeration", "suffix")}>
                <Input placeholder="Suffix" />
              </Form.Item>
            </Input.Group>
          </Form.Item>
          <Form.Item label="Year" name={namespace.concat("reference", "standard", "year")}>
            <InputNumber />
          </Form.Item>
          <Form.Item
            label="Locator"
            name={namespace.concat("reference", "standard", "locator")}
            rules={[
              {
                type: "url",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item label="Standardizers" name={namespace.concat("reference", "standard", "standardizers")}>
            <Select
              mode="multiple"
              placeholder="Please select"
              options={Object.entries(Standardizer).map(([_key, value]) => ({
                label: value,
                value: value,
              }))}
            />
          </Form.Item>
        </>
      )}
    </>
  );
}
