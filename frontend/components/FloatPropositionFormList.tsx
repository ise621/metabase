import { Form, Input, Select, InputNumber, Button } from "antd";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";

const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

enum Negator {
  Is = "is",
  IsNot = "isNot",
}

export enum FloatPropositionComparator {
  EqualTo = "equalTo",
  LessThanOrEqualTo = "lessThanOrEqualTo",
  GreaterThanOrEqualTo = "greaterThanOrEqualTo",
  // InClosedInterval = "inClosedInterval"
}

export type FloatPropositionFormListProps = {
  name: string;
  label: string;
  minimum: number;
  maximum: number;
};

export function FloatPropositionFormList({
  name,
  label,
  minimum,
  maximum,
}: FloatPropositionFormListProps) {
  return (
    <Form.List name={name}>
      {(fields, { add, remove }, { errors }) => (
        <>
          {fields.map(({ key, name, fieldKey, ...restField }, index) => (
            <Form.Item key={key} label={index === 0 ? label : " "}>
              <Input.Group>
                <Form.Item
                  {...restField}
                  key={`negator${key}`}
                  name={[name, "negator"]}
                  fieldKey={[fieldKey ?? -1, "negator"]}
                  noStyle
                  initialValue={Negator.Is}
                >
                  <Select
                    style={{ width: "10%" }}
                    options={[
                      { label: "Is", value: Negator.Is },
                      { label: "Is not", value: Negator.IsNot },
                    ]}
                  />
                </Form.Item>
                <Form.Item
                  {...restField}
                  key={`comparator${key}`}
                  name={[name, "comparator"]}
                  fieldKey={[fieldKey ?? -1, "comparator"]}
                  noStyle
                  initialValue={FloatPropositionComparator.EqualTo}
                >
                  <Select
                    style={{ width: "20%" }}
                    options={[
                      {
                        label: "equal to",
                        value: FloatPropositionComparator.EqualTo,
                      },
                      {
                        label: "greater than or equal to",
                        value: FloatPropositionComparator.GreaterThanOrEqualTo,
                      },
                      {
                        label: "less than or equal to",
                        value: FloatPropositionComparator.LessThanOrEqualTo,
                      },
                      // {
                      //     label: "in closed interval",
                      //     value: FloatPropositionComparator.InClosedInterval
                      // }
                    ]}
                  />
                </Form.Item>
                <Form.Item
                  {...restField}
                  key={`value${key}`}
                  name={[name, "value"]}
                  fieldKey={[fieldKey ?? -1, "value"]}
                  noStyle
                >
                  <InputNumber
                    min={minimum}
                    max={maximum}
                    step="0.01"
                    style={{ width: "60%" }}
                  />
                </Form.Item>
                <MinusCircleOutlined
                  style={{ width: "10%" }}
                  onClick={() => remove(name)}
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
              {`Add ${label.toLocaleLowerCase("en")} proposition`}
            </Button>
            <Form.ErrorList errors={errors} />
          </Form.Item>
        </>
      )}
    </Form.List>
  );
}
