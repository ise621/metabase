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

function removeTypenames(
  reference: Publication | Standard | null | undefined
): Publication | Standard | null {
  if (reference == null) {
    return null;
  }
  const { __typename, ...referenceWithoutTypename } = reference;
  if ("numeration" in referenceWithoutTypename) {
    const { __typename, ...numerationWithoutTypename } = referenceWithoutTypename.numeration;
    referenceWithoutTypename.numeration = numerationWithoutTypename;
  }
  return referenceWithoutTypename;
}

// interface HasStandardAndPublication {
//   standard: CreateStandardInput | null | undefined;
//   publication: CreatePublicationInput | null | undefined;
// }

export type ReferenceFormProps<Values> = {
  form: FormInstance<Values>;
  initialValue?: Standard | Publication | null;
  namespace: string[];
};

// TODO Why does the following not work? export function ReferenceForm<Values extends HasStandardAndPublication>({form}: ReferenceFormProps<Values>) {
export function ReferenceForm({ form, initialValue, namespace }: ReferenceFormProps<any>) {
  const initialKind = referenceToKind(initialValue);
  const initialReference = removeTypenames(initialValue);
  const [selectedReferenceOption, setSelectedReferenceOption] =
    useState(initialKind);

  const onReferenceChange = (value: ReferenceKind) => {
    if (value != selectedReferenceOption) {
      switch (value) {
        case ReferenceKind.None:
          form.setFieldValue(namespace.concat("publication"), null);
          form.setFieldValue(namespace.concat("standard"), null);
          break;
        case ReferenceKind.Publication:
          form.setFieldValue(namespace.concat("standard"), null);
          if (initialKind == ReferenceKind.Publication) {
            form.setFieldValue(namespace.concat("publication"), initialReference);
          }
          break;
        case ReferenceKind.Standard:
          form.setFieldValue(namespace.concat("publication"), null);
          if (initialKind == ReferenceKind.Standard) {
            form.setFieldValue(namespace.concat("standard"), initialReference);
          }
          break;
        default:
          console.error("Impossible!");
      }
      setSelectedReferenceOption(value);
    }
  };

  return (
    <>
      <Form.Item label="Reference" name={["unmapped"].concat(namespace)} initialValue={initialKind}>
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
          <Form.Item label="Title" name={namespace.concat("publication", "title")} initialValue={initialValue?.__typename == "Publication" ? initialValue.title : null}>
            <Input />
          </Form.Item>
          <Form.Item label="Abstract" name={namespace.concat("publication", "abstract")} initialValue={initialValue?.__typename == "Publication" ? initialValue.abstract : null}>
            <Input />
          </Form.Item>
          <Form.Item label="Section" name={namespace.concat("publication", "section")} initialValue={initialValue?.__typename == "Publication" ? initialValue.section : null}>
            <Input />
          </Form.Item>
          <Form.Item label="arXiv" name={namespace.concat("publication", "arXiv")} initialValue={initialValue?.__typename == "Publication" ? initialValue.arXiv : null}>
            <Input />
          </Form.Item>
          <Form.Item label="DOI" name={namespace.concat("publication", "doi")} initialValue={initialValue?.__typename == "Publication" ? initialValue.doi : null}>
            <Input />
          </Form.Item>
          <Form.Item label="URN" name={namespace.concat("publication", "urn")} initialValue={initialValue?.__typename == "Publication" ? initialValue.urn : null}>
            <Input />
          </Form.Item>
          <Form.Item
            label="WebAddress"
            name={namespace.concat("publication", "webAddress")}
            initialValue={initialValue?.__typename == "Publication" ? initialValue.webAddress : null}
            rules={[
              {
                type: "url",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.List name={namespace.concat("publication", "authors")} initialValue={initialValue?.__typename == "Publication" ? (initialValue.authors == null ? undefined : initialValue.authors) : undefined}>
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
          <Form.Item label="Title" name={namespace.concat("standard", "title")} initialValue={initialValue?.__typename == "Standard" ? initialValue.title : null}>
            <Input />
          </Form.Item>
          <Form.Item label="Abstract" name={namespace.concat("standard", "abstract")} initialValue={initialValue?.__typename == "Standard" ? initialValue.abstract : null}>
            <Input />
          </Form.Item>
          <Form.Item label="Section" name={namespace.concat("standard", "section")} initialValue={initialValue?.__typename == "Standard" ? initialValue.section : null}>
            <Input />
          </Form.Item>
          <Form.Item label="Numeration">
            <Input.Group>
              <Form.Item
                noStyle
                name={namespace.concat("standard", "numeration", "mainNumber")}
                initialValue={initialValue?.__typename == "Standard" ? initialValue.numeration.mainNumber : null}
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Input placeholder="Main Number" />
              </Form.Item>
              <Form.Item noStyle name={namespace.concat("standard", "numeration", "prefix")} initialValue={initialValue?.__typename == "Standard" ? initialValue.numeration.prefix : null}>
                <Input placeholder="Prefix" />
              </Form.Item>
              <Form.Item noStyle name={namespace.concat("standard", "numeration", "suffix")} initialValue={initialValue?.__typename == "Standard" ? initialValue.numeration.suffix : null}>
                <Input placeholder="Suffix" />
              </Form.Item>
            </Input.Group>
          </Form.Item>
          <Form.Item label="Year" name={namespace.concat("standard", "year")} initialValue={initialValue?.__typename == "Standard" ? initialValue.year : null}>
            <InputNumber />
          </Form.Item>
          <Form.Item
            label="Locator"
            name={namespace.concat("standard", "locator")}
            initialValue={initialValue?.__typename == "Standard" ? initialValue.locator : null}
            rules={[
              {
                type: "url",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item label="Standardizers" name={namespace.concat("standard", "standardizers")} initialValue={initialValue?.__typename == "Standard" ? initialValue.standardizers : null}>
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
