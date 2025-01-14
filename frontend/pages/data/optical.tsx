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
import { useAllOpticalDataQuery } from "../../queries/data.graphql";
import {
  Scalars,
  OpticalDataPropositionInput,
} from "../../__generated__/__types__";
import { useState } from "react";
import Link from "next/link";
import paths from "../../paths";
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
  proposition: OpticalDataPropositionInput
): OpticalDataPropositionInput => {
  switch (negator) {
    case Negator.Is:
      return proposition;
    case Negator.IsNot:
      return { not: proposition };
  }
};

const conjunct = (
  propositions: OpticalDataPropositionInput[]
): OpticalDataPropositionInput => {
  if (propositions.length == 0) {
    return {};
  }
  if (propositions.length == 1) {
    return propositions[0];
  }
  return { and: propositions };
};

// const disjunct = (
//   propositions: OpticalDataPropositionInput[]
// ): OpticalDataPropositionInput => {
//   if (propositions.length == 0) {
//     return {};
//   }
//   if (propositions.length == 1) {
//     return propositions[0];
//   }
//   return { or: propositions };
// };

type PartialOpticalData = {
  __typename?: "OpticalData";
  infraredEmittances: Array<number>;
  nearnormalHemisphericalSolarReflectances: Array<number>;
  nearnormalHemisphericalSolarTransmittances: Array<number>;
  nearnormalHemisphericalVisibleReflectances: Array<number>;
  nearnormalHemisphericalVisibleTransmittances: Array<number>;
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
  const [data, setData] = useState<PartialOpticalData[]>([]);
  // Using `skip` is inspired by https://github.com/apollographql/apollo-client/issues/5268#issuecomment-749501801
  // An alternative would be `useLazy...` as told in https://github.com/apollographql/apollo-client/issues/5268#issuecomment-527727653
  // `useLazy...` does not return a `Promise` though as `use...Query.refetch` does which is used below.
  // For error policies see https://www.apollographql.com/docs/react/v2/data/error-handling/#error-policies
  const allOpticalDataQuery = useAllOpticalDataQuery({
    skip: true,
    errorPolicy: "all",
  });

  const [filterText, setFilterText] = useState(() => new Map<string, string>());
  const onFilterTextChange = setMapValue(filterText, setFilterText);

  const onFinish = ({
    componentIds,
    dataFormatIds,
    infraredEmittances,
    nearnormalHemisphericalSolarReflectances,
    nearnormalHemisphericalSolarTransmittances,
    nearnormalHemisphericalVisibleReflectances,
    nearnormalHemisphericalVisibleTransmittances,
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
    infraredEmittances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalSolarReflectances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalSolarTransmittances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalVisibleReflectances:
      | {
          negator: Negator;
          comparator: FloatPropositionComparator;
          value: number | undefined;
        }[]
      | undefined;
    nearnormalHemisphericalVisibleTransmittances:
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
        const propositions: OpticalDataPropositionInput[] = [];
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
        // `if (nearnormalHemisphericalVisibleTransmittance)`.
        if (infraredEmittances) {
          for (let { negator, comparator, value } of infraredEmittances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  infraredEmittances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        if (nearnormalHemisphericalSolarReflectances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalSolarReflectances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalSolarReflectances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        if (nearnormalHemisphericalSolarTransmittances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalSolarTransmittances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalSolarTransmittances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        if (nearnormalHemisphericalVisibleReflectances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalVisibleReflectances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalVisibleReflectances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        if (nearnormalHemisphericalVisibleTransmittances) {
          for (let {
            negator,
            comparator,
            value,
          } of nearnormalHemisphericalVisibleTransmittances) {
            if (value !== undefined && value !== null) {
              propositions.push(
                negateIfNecessary(negator, {
                  nearnormalHemisphericalVisibleTransmittances: {
                    some: {
                      [comparator]: value,
                    },
                  },
                })
              );
            }
          }
        }
        const { error, data } = await allOpticalDataQuery.refetch(
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
        // TODO Add `edge.node.databaseId to nodes?
        const nestedData =
          data?.databases?.edges?.map(
            (edge) => edge?.node?.allOpticalData?.edges?.map((e) => e.node) || []
          ) || [];
        const flatData = ([] as PartialOpticalData[]).concat(...nestedData);
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
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The network of <Link href={paths.databases}>databases</Link> can be
        queried here for data on building envelope{" "}
        <Link href={paths.components}>components</Link>.
      </Typography.Paragraph>
      <Typography.Title>Optical Data</Typography.Title>
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
        <FloatPropositionFormList
          name="infraredEmittances"
          label="Infrared emittance"
          minimum={0}
          maximum={1}
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalSolarReflectances"
          label="Nearnormal hemispherical solar reflectance"
          minimum={0}
          maximum={1}
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalSolarTransmittances"
          label="Nearnormal hemispherical solar transmittance"
          minimum={0}
          maximum={1}
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalVisibleReflectances"
          label="Nearnormal hemispherical visible reflectance"
          minimum={0}
          maximum={1}
        />
        <FloatPropositionFormList
          name="nearnormalHemisphericalVisibleTransmittances"
          label="Nearnormal hemispherical visible transmittance"
          minimum={0}
          maximum={1}
        />

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
            title: "Emittances, Reflectances, and Transmittances",
            key: "nearnormalHemisphericalX",
            render: (_text, record, _index) => (
              <Descriptions column={1}>
                <Descriptions.Item
                  key="infraredEmittances"
                  label="Infrared Emittances"
                >
                  {record.infraredEmittances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalSolarReflectances"
                  label="Nearnormal Hemispherical Solar Reflectances"
                >
                  {record.nearnormalHemisphericalSolarReflectances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalSolarTransmittances"
                  label="Nearnormal Hemispherical Solar Transmittances"
                >
                  {record.nearnormalHemisphericalSolarTransmittances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalVisibleReflectances"
                  label="Nearnormal Hemispherical Visible Reflectances"
                >
                  {record.nearnormalHemisphericalVisibleReflectances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
                <Descriptions.Item
                  key="nearnormalHemisphericalVisibleTransmittances"
                  label="Nearnormal Hemispherical Visible Transmittances"
                >
                  {record.nearnormalHemisphericalVisibleTransmittances
                    .map((x) => x.toLocaleString("en"))
                    .join(", ")}
                </Descriptions.Item>
              </Descriptions>
            ),
          },
        ]}
        dataSource={data}
      />
      <Typography.Paragraph style={{ maxWidth: 768 }}>
        The{" "}
        <Typography.Link
          href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
        >
          GraphQL endpoint
        </Typography.Link>{" "}
        is the most powerful way of querying the databases.
      </Typography.Paragraph>
    </Layout>
  );
}

export default Page;
