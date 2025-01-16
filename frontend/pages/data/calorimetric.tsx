import Layout from "../../components/Layout";
import {
  Table,
  message,
  Form,
  Button,
  Alert,
  Typography,
  Descriptions,
} from "antd";
import { useAllCalorimetricDataQuery } from "../../queries/data.graphql";
import {
  Scalars,
  CalorimetricDataPropositionInput,
} from "../../__generated__/__types__";
import { useState } from "react";
import { setMapValue } from "../../lib/freeTextFilter";
import {
  getAppliedMethodColumnProps,
  getComponentUuidColumnProps,
  getDescriptionColumnProps,
  getNameColumnProps,
  getResourceTreeColumnProps,
  getTimestampColumnProps,
  getUuidColumnProps,
} from "../../lib/table";
import {
  FloatPropositionComparator,
  FloatPropositionFormList,
} from "../../components/FloatPropositionFormList";
import {
  UuidPropositionComparator,
  UuidPropositionFormList,
} from "../../components/UuidPropositionFormList";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

enum Negator {
  Is = "is",
  IsNot = "isNot",
}

const negateIfNecessary = (
  negator: Negator,
  proposition: CalorimetricDataPropositionInput
): CalorimetricDataPropositionInput => {
  switch (negator) {
    case Negator.Is:
      return proposition;
    case Negator.IsNot:
      // return { not: proposition };
      return proposition; // TODO Support `not` in filter!
  }
};

const conjunct = (
  propositions: CalorimetricDataPropositionInput[]
): CalorimetricDataPropositionInput => {
  if (propositions.length == 0) {
    return {};
  }
  if (propositions.length == 1) {
    return propositions[0];
  }
  return { and: propositions };
};

// const disjunct = (
//   propositions: CalorimetricDataPropositionInput[]
// ): CalorimetricDataPropositionInput => {
//   if (propositions.length == 0) {
//     return {};
//   }
//   if (propositions.length == 1) {
//     return propositions[0];
//   }
//   return { or: propositions };
// };

type PartialCalorimetricData = {
  __typename?: "CalorimetricData";
  gValues: Array<number>;
  uValues: Array<number>;
  uuid: Scalars["Uuid"];
  timestamp: Scalars["DateTime"];
  componentId: Scalars["Uuid"];
  name?: string | null | undefined;
  description?: string | null | undefined;
  appliedMethod: {
    __typename?: "AppliedMethod";
    methodId: Scalars["Uuid"];
  };
  resourceTree: {
    __typename?: "GetHttpsResourceTree";
    root: {
      __typename?: "GetHttpsResourceTreeRoot";
      value: {
        __typename?: "GetHttpsResource";
        description: string;
        hashValue: string;
        locator: Scalars["Url"];
        dataFormatId: Scalars["Uuid"];
      };
    };
  };
};

function Page() {
  const [form] = Form.useForm();
  const [filtering, setFiltering] = useState(false);
  const [globalErrorMessages, setGlobalErrorMessages] = useState(
    new Array<string>()
  );
  const [data, setData] = useState<PartialCalorimetricData[]>([]);
  // Using `skip` is inspired by https://github.com/apollographql/apollo-client/issues/5268#issuecomment-749501801
  // An alternative would be `useLazy...` as told in https://github.com/apollographql/apollo-client/issues/5268#issuecomment-527727653
  // `useLazy...` does not return a `Promise` though as `use...Query.refetch` does which is used below.
  // For error policies see https://www.apollographql.com/docs/react/v2/data/error-handling/#error-policies
  const allCalorimetricDataQuery = useAllCalorimetricDataQuery({
    skip: true,
    errorPolicy: "all",
  });

  const [filterText, setFilterText] = useState(() => new Map<string, string>());
  const onFilterTextChange = setMapValue(filterText, setFilterText);

  const onFinish = ({
    componentIds,
    gValues,
    uValues,
    dataFormatIds,
  }: {
    componentIds:
      | {
          negator: Negator;
          comparator: UuidPropositionComparator;
          value: Scalars["Uuid"] | undefined;
        }[]
      | undefined;
    dataFormatIds:
      | {
          negator: Negator;
          comparator: UuidPropositionComparator;
          value: Scalars["Uuid"] | undefined;
        }[]
      | undefined;
    gValues:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    uValues:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
  }) => {
    const filter = async () => {
      try {
        setFiltering(true);
        // https://www.apollographql.com/docs/react/networking/authentication/#reset-store-on-logout
        const propositions: CalorimetricDataPropositionInput[] = [];
        if (componentIds) {
          for (let { negator, comparator, value } of componentIds) {
            propositions.push(
              negateIfNecessary(negator, {
                componentId: { [comparator]: value },
              })
            );
          }
        }
        if (dataFormatIds) {
          for (let { negator, comparator, value } of dataFormatIds) {
            propositions.push(
              negateIfNecessary(negator, {
                resources: {
                  some: {
                    dataFormatId: { [comparator]: value },
                  },
                },
              })
            );
          }
        }
        // Note that `0` evaluates to `false`, so below we cannot use
        // `if (value)`.
        if (gValues) {
          for (let { negator, comparator, value } of gValues) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  gValues: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        if (uValues) {
          for (let { negator, comparator, value } of uValues) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  uValues: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        const { error, data } = await allCalorimetricDataQuery.refetch(
          propositions.length == 0
            ? {}
            : {
                where: conjunct(propositions),
              }
        );
        if (error) {
          // TODO Handle properly.
          console.log(error);
          message.error(
            error.graphQLErrors.map((error) => error.message).join(" ")
          );
        }
        const nestedData =
          data?.databases?.edges?.map(
            (edge) => edge?.node?.allCalorimetricData?.edges?.map((e) => e.node) || []
          ) || [];
        const flatData = ([] as PartialCalorimetricData[]).concat(
          ...nestedData
        );
        setData(flatData);
      } catch (error) {
        // TODO Handle properly.
        console.log("Failed:", error);
      } finally {
        setFiltering(false);
      }
    };
    filter();
  };

  const onFinishFailed = () => {
    setGlobalErrorMessages(["Fix the errors below."]);
  };

  return (
    <Layout>
      <Typography.Title>Calorimetric Data</Typography.Title>
      {/* TODO Display error messages in a list? */}
      {globalErrorMessages.length > 0 && (
        <Alert type="error" message={globalErrorMessages.join(" ")} />
      )}
      <Form
        {...layout}
        form={form}
        name="filterData"
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
      >
        <UuidPropositionFormList name="componentIds" label="Component Id" />
        <UuidPropositionFormList name="dataFormatIds" label="Data Format Id" />
        <FloatPropositionFormList name="gValues" label="g Value" minimum={0} maximum={1} />
        <FloatPropositionFormList name="uValues" label="u Value" minimum={0} maximum={1} />

        <Form.Item {...tailLayout}>
          <Button type="primary" htmlType="submit" loading={filtering}>
            Filter
          </Button>
        </Form.Item>
      </Form>
      <Table
        loading={filtering}
        columns={[
          {
            ...getUuidColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x),
              (_uuid) => "/" // TODO Link somewhere useful!
            ),
          },
          {
            ...getNameColumnProps<(typeof data)[0]>(onFilterTextChange, (x) =>
              filterText.get(x)
            ),
          },
          {
            ...getDescriptionColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x)
            ),
          },
          {
            ...getTimestampColumnProps<(typeof data)[0]>(),
          },
          {
            ...getComponentUuidColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x)
            ),
          },
          // {
          //   ...getInternallyLinkedFilterableStringColumnProps<typeof data[0]>(
          //     "Database UUID",
          //     "databaseId",
          //     (x) => x.databaseId,
          //     onFilterTextChange,
          //     (x) => filterText.get(x),
          //     (x) => paths.database(x.databaseId)
          //   ),
          // },
          {
            ...getAppliedMethodColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x)
            ),
          },
          {
            ...getResourceTreeColumnProps<(typeof data)[0]>(
              onFilterTextChange,
              (x) => filterText.get(x)
            ),
          },
          {
            title: "g and u Values",
            key: "g-and-u-values",
            render: (_text, record, _index) => (
              <Descriptions column={1}>
                <Descriptions.Item key="gValues" label="g Values">
                  {record.gValues.map((x) => x.toLocaleString("en")).join(", ")}
                </Descriptions.Item>
                <Descriptions.Item key="uValues" label="u Values">
                  {record.uValues.map((x) => x.toLocaleString("en")).join(", ")}
                </Descriptions.Item>
              </Descriptions>
            ),
          },
        ]}
        dataSource={data}
      />
    </Layout>
  );
}

export default Page;
