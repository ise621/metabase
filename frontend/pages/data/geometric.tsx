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
import { useAllGeometricDataQuery } from "../../queries/data.graphql";
import {
    Scalars,
    GeometricDataPropositionInput,
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
    proposition: GeometricDataPropositionInput
): GeometricDataPropositionInput => {
    switch (negator) {
        case Negator.Is:
            return proposition;
        case Negator.IsNot:
            return proposition;
    }
};

const conjunct = (
    propositions: GeometricDataPropositionInput[]
): GeometricDataPropositionInput => {
    if (propositions.length == 0) {
        return {};
    }
    if (propositions.length == 1) {
        return propositions[0];
    }
    return { and: propositions };
};

type PartialGeometricData = {
    __typename?: "GeometricData";
    thicknesses: Array<number>;
    uuid: any;
    timestamp: any;
    componentId: any;
    name?: string | null | undefined;
    description?: string | null | undefined;
    appliedMethod: {
        __typename?: "AppliedMethod";
        methodId: any;
    };
    resourceTree: {
        __typename?: "GetHttpsResourceTree";
        root: {
            __typename?: "GetHttpsResourceTreeRoot";
            value: {
                __typename?: "GetHttpsResource";
                description: string;
                hashValue: string;
                locator: any;
                dataFormatId: any;
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
    const [data, setData] = useState<PartialGeometricData[]>([]);

    const allGeometricDataQuery = useAllGeometricDataQuery({
        skip: true,
        errorPolicy: "all",
    });
    const [filterText, setFilterText] = useState(() => new Map<string, string>());
    const onFilterTextChange = setMapValue(filterText, setFilterText);

    const onFinish = ({
        componentIds,
        thicknesses,
        dataFormatIds
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
        thicknesses:
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
                const propositions: GeometricDataPropositionInput[] = [];
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
                if (thicknesses) {
                    for (let { negator, comparator, value } of thicknesses) {
                        if (value !== undefined && value !== null) {
                            propositions.push(
                                negateIfNecessary(negator, {
                                    thicknesses: {
                                        some: {
                                            [comparator]: value,
                                        },
                                    },
                                })
                            );
                        }
                    }
                }
                const { error, data } = await allGeometricDataQuery.refetch(
                    propositions.length == 0
                        ? {}
                        : {
                            where: conjunct(propositions),
                        }
                );
                if (error) {
                    console.log(error);
                    message.error(
                        error.graphQLErrors.map((error) => error.message).join(" ")
                    );
                }
                const nestedData =
                    data?.databases?.edges?.map(
                        (edge) => edge?.node?.allGeometricData?.edges?.map((e) => e.node) || []
                    ) || [];
                const flatData = ([] as PartialGeometricData[]).concat(
                    ...nestedData
                );
                setData(flatData);
            } catch (error) {
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
            <Typography.Title>Geometric Data</Typography.Title>
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
                <FloatPropositionFormList name="thicknesses" label="Thickness" minimum={0} maximum={Infinity} />

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
                            (_uuid) => "/"
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
                        title: "Thicknesses",
                        key: "thicknesses",
                        render: (_text, record, _index) => (
                            <Descriptions column={1}>
                                <Descriptions.Item key="thicknesses" label="Thicknesses">
                                    {record.thicknesses.map((x) => x.toLocaleString("en")).join(", ")}
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