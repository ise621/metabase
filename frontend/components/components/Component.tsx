import { Scalars } from "../../__generated__/__types__";
import {
  ComponentDocument,
  ComponentsDocument,
  useComponentQuery,
} from "../../queries/components.graphql";
import { useRemoveComponentAssemblyMutation } from "../../queries/componentAssemblies.graphql";
import { useRemoveComponentVariantMutation } from "../../queries/componentVariants.graphql";
import {
  Skeleton,
  Result,
  Descriptions,
  Tag,
  List,
  Button,
  message,
  Row,
  Col,
  Space,
} from "antd";
import { PageHeader } from "@ant-design/pro-layout";
import { ReactNode, useEffect, useState } from "react";
import paths from "../../paths";
import Link from "next/link";
import OpenEndedDateTimeRangeX from "../OpenEndedDateTimeRangeX";
import { messageApolloError } from "../../lib/apollo";
import AddPartOfComponent from "./AddPartOfComponent";
import AddAssembledOfComponent from "./AddAssembledOfComponent";
import UpdateComponentAssembly from "./UpdateComponentAssembly";
import AddVariantOfComponent from "./AddVariantOfComponent";
import AddConcretizationOfComponent from "./AddConcretizationOfComponent";
import AddGeneralizationOfComponent from "./AddGeneralizationOfComponent";
import { useRemoveComponentGeneralizationMutation } from "../../queries/componentGeneralizations.graphql";
import UpdateComponent from "./UpdateComponent";
import AddComponentManufacturer from "./AddComponentManufacturer";
import { useRemoveComponentManufacturerMutation } from "../../queries/componentManufacturers.graphql";
import { InstitutionDocument } from "../../queries/institutions.graphql";

export type ComponentProps = {
  componentId: Scalars["Uuid"];
};

export default function Component({ componentId }: ComponentProps) {
  const { loading, error, data } = useComponentQuery({
    variables: {
      uuid: componentId,
    },
  });
  const component = data?.component;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  const [removeComponentManufacturerMutation] =
    useRemoveComponentManufacturerMutation();
  const [removingComponentManufacturer, setRemovingComponentManufacturer] =
    useState(false);

  const removeComponentManufacturer = async (
    institutionId: Scalars["Uuid"]
  ) => {
    try {
      setRemovingComponentManufacturer(true);
      const { errors, data } = await removeComponentManufacturerMutation({
        variables: {
          componentId: componentId,
          institutionId: institutionId,
        },
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
          {
            query: InstitutionDocument,
            variables: {
              uuid: institutionId,
            },
          },
        ],
      });
      if (errors) {
        console.log(errors); // TODO What to do?
      } else if (data?.removeComponentManufacturer?.errors) {
        // TODO Is this how we want to display errors?
        message.error(
          data?.removeComponentManufacturer?.errors
            .map((error) => error.message)
            .join(" ")
        );
      }
    } finally {
      setRemovingComponentManufacturer(false);
    }
  };

  const [removeComponentAssemblyMutation] =
    useRemoveComponentAssemblyMutation();
  const [removingComponentAssembly, setRemovingComponentAssembly] =
    useState(false);

  const removeComponentAssembly = async (
    assembledComponentId: Scalars["Uuid"],
    partComponentId: Scalars["Uuid"]
  ) => {
    try {
      setRemovingComponentAssembly(true);
      const { errors, data } = await removeComponentAssemblyMutation({
        variables: {
          assembledComponentId: assembledComponentId,
          partComponentId: partComponentId,
        },
        refetchQueries: [
          {
            query: ComponentsDocument,
          },
          {
            query: ComponentDocument,
            variables: {
              uuid: assembledComponentId,
            },
          },
          {
            query: ComponentDocument,
            variables: {
              uuid: partComponentId,
            },
          },
        ],
      });
      if (errors) {
        console.log(errors); // TODO What to do?
      } else if (data?.removeComponentAssembly?.errors) {
        // TODO Is this how we want to display errors?
        message.error(
          data?.removeComponentAssembly?.errors
            .map((error) => error.message)
            .join(" ")
        );
      }
    } finally {
      setRemovingComponentAssembly(false);
    }
  };

  const [removeComponentVariantMutation] = useRemoveComponentVariantMutation();
  const [removingComponentVariant, setRemovingComponentVariant] =
    useState(false);

  const removeComponentVariant = async (
    oneComponentId: Scalars["Uuid"],
    otherComponentId: Scalars["Uuid"]
  ) => {
    try {
      setRemovingComponentVariant(true);
      const { errors, data } = await removeComponentVariantMutation({
        variables: {
          oneComponentId: oneComponentId,
          otherComponentId: otherComponentId,
        },
        refetchQueries: [
          {
            query: ComponentsDocument,
          },
          {
            query: ComponentDocument,
            variables: {
              uuid: oneComponentId,
            },
          },
          {
            query: ComponentDocument,
            variables: {
              uuid: otherComponentId,
            },
          },
        ],
      });
      if (errors) {
        console.log(errors); // TODO What to do?
      } else if (data?.removeComponentVariant?.errors) {
        // TODO Is this how we want to display errors?
        message.error(
          data?.removeComponentVariant?.errors
            .map((error) => error.message)
            .join(" ")
        );
      }
    } finally {
      setRemovingComponentVariant(false);
    }
  };

  const [removeComponentGeneralizationMutation] =
    useRemoveComponentGeneralizationMutation();
  const [removingComponentGeneralization, setRemovingComponentGeneralization] =
    useState(false);

  const removeComponentGeneralization = async (
    generalComponentId: Scalars["Uuid"],
    concreteComponentId: Scalars["Uuid"]
  ) => {
    try {
      setRemovingComponentGeneralization(true);
      const { errors, data } = await removeComponentGeneralizationMutation({
        variables: {
          generalComponentId: generalComponentId,
          concreteComponentId: concreteComponentId,
        },
        refetchQueries: [
          {
            query: ComponentsDocument,
          },
          {
            query: ComponentDocument,
            variables: {
              uuid: generalComponentId,
            },
          },
          {
            query: ComponentDocument,
            variables: {
              uuid: concreteComponentId,
            },
          },
        ],
      });
      if (errors) {
        console.log(errors); // TODO What to do?
      } else if (data?.removeComponentGeneralization?.errors) {
        // TODO Is this how we want to display errors?
        message.error(
          data?.removeComponentGeneralization?.errors
            .map((error) => error.message)
            .join(" ")
        );
      }
    } finally {
      setRemovingComponentGeneralization(false);
    }
  };

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!component) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return <>
    <PageHeader
      title={[
        component.name,
        component.abbreviation == null ? null : `(${component.abbreviation})`,
      ]
        .filter((x) => x != null)
        .join(" ")}
      subTitle={component.description}
      tags={component.categories.map((x) => (
        <Tag key={x} color="magenta">
          {x}
        </Tag>
      ))}
      extra={
        component.canCurrentUserUpdateNode
          ? [
              <UpdateComponent
                key="updateComponent"
                componentId={component.uuid}
                name={component.name}
                abbreviation={component.abbreviation}
                description={component.description}
                availability={component.availability}
                categories={component.categories}
                primeSurface={component.prime?.surface}
                primeDirection={component.prime?.direction}
                switchableLayers={component.switchableLayers}
              />,
            ]
          : []
      }
      backIcon={false}
    >
      <Descriptions size="small" column={1}>
        <Descriptions.Item label="UUID">{component.uuid}</Descriptions.Item>
        <Descriptions.Item label="Available">
          <OpenEndedDateTimeRangeX range={component.availability} />
        </Descriptions.Item>
        <Descriptions.Item label="Prime Surface">
          {component.prime?.surface?.description}{" "}
          {component.prime?.surface?.reference?.title}
        </Descriptions.Item>
        <Descriptions.Item label="Prime Direction">
          {component.prime?.direction?.description}{" "}
          {component.prime?.direction?.reference?.title}
        </Descriptions.Item>
        <Descriptions.Item label="Switchable Layers">
          {component.switchableLayers?.description}{" "}
          {component.switchableLayers?.reference?.title}
        </Descriptions.Item>
      </Descriptions>
    </PageHeader>
    <Space direction="vertical" style={{ display: "flex" }}>
      <Row gutter={[16, 16]}>
        <Col flex={1}>
          <List
            header="Manufacturers"
            bordered={true}
            size="small"
            footer={
              component.manufacturers.canCurrentUserAddEdge && (
                <AddComponentManufacturer componentId={component.uuid} />
              )
            }
          >
            {component.manufacturers.edges.map((x) => (
              <List.Item
                key={x.node.uuid}
                actions={([] as ReactNode[]).concat(
                  x.canCurrentUserRemoveEdge
                    ? [
                        <Button
                          key="remove"
                          onClick={() =>
                            removeComponentManufacturer(x.node.uuid)
                          }
                          loading={removingComponentManufacturer}
                        >
                          Remove
                        </Button>,
                      ]
                    : []
                )}
              >
                <List.Item.Meta
                  title={
                    <Link href={paths.institution(x.node.uuid)} legacyBehavior>
                      {x.node.name}
                    </Link>
                  }
                  description={x.node.description}
                />
              </List.Item>
            ))}
            {component.pendingManufacturers.edges.map((x) => (
              <List.Item
                key={x.node.uuid}
                actions={([] as ReactNode[]).concat(
                  x.canCurrentUserRemoveEdge
                    ? [
                        <Button
                          key="remove"
                          onClick={() =>
                            removeComponentManufacturer(x.node.uuid)
                          }
                          loading={removingComponentManufacturer}
                        >
                          Remove
                        </Button>,
                      ]
                    : []
                )}
              >
                <List.Item.Meta
                  title={
                    <Link href={paths.institution(x.node.uuid)} legacyBehavior>
                      {x.node.name} (Pending)
                    </Link>
                  }
                />
              </List.Item>
            ))}
          </List>
        </Col>
      </Row>
      {(component.assembledOf.edges.length >= 1 ||
        component.assembledOf.canCurrentUserAddEdge ||
        component.partOf.edges.length >= 1 ||
        component.partOf.canCurrentUserAddEdge) && (
        <Row gutter={[16, 16]}>
          <Col flex={1}>
            {(component.assembledOf.edges.length >= 1 ||
              component.assembledOf.canCurrentUserAddEdge) && (
              <List
                header="Assembled Of"
                bordered={true}
                size="small"
                footer={
                  component.assembledOf.canCurrentUserAddEdge && (
                    <AddPartOfComponent
                      assembledComponentId={component.uuid}
                    />
                  )
                }
              >
                {component.assembledOf.edges.map((x) => (
                  <List.Item
                    key={x.node.uuid}
                    actions={([] as ReactNode[])
                      .concat(
                        x.canCurrentUserUpdateEdge
                          ? [
                              <UpdateComponentAssembly
                                key="update"
                                assembledComponent={{
                                  uuid: component.uuid,
                                  name: component.name,
                                }}
                                partComponent={{
                                  uuid: x.node.uuid,
                                  name: x.node.name,
                                }}
                                index={x.index}
                                primeSurface={x.primeSurface}
                              />,
                            ]
                          : []
                      )
                      .concat(
                        x.canCurrentUserRemoveEdge
                          ? [
                              <Button
                                key="remove"
                                onClick={() =>
                                  removeComponentAssembly(
                                    component.uuid,
                                    x.node.uuid
                                  )
                                }
                                loading={removingComponentAssembly}
                              >
                                Remove
                              </Button>,
                            ]
                          : []
                      )}
                  >
                    <List.Item.Meta
                      title={
                        <Space>
                          <Link href={paths.component(x.node.uuid)} legacyBehavior>
                            {x.node.name}
                          </Link>
                          <div>
                            <Tag color="purple">Layer {x.index}</Tag>
                            <Tag color="volcano">
                              Prime Surface {x.primeSurface}
                            </Tag>
                          </div>
                        </Space>
                      }
                      description={x.node.description}
                    />
                  </List.Item>
                ))}
              </List>
            )}
          </Col>
          <Col flex={1}>
            {(component.partOf.edges.length >= 1 ||
              component.partOf.canCurrentUserAddEdge) && (
              <List
                header="Part Of"
                bordered={true}
                size="small"
                footer={
                  component.partOf.canCurrentUserAddEdge && (
                    <AddAssembledOfComponent
                      partComponentId={component.uuid}
                    />
                  )
                }
              >
                {component.partOf.edges.map((x) => (
                  <List.Item
                    key={x.node.uuid}
                    actions={([] as ReactNode[])
                      .concat(
                        x.canCurrentUserUpdateEdge
                          ? [
                              <UpdateComponentAssembly
                                key="update"
                                assembledComponent={{
                                  uuid: x.node.uuid,
                                  name: x.node.name,
                                }}
                                partComponent={{
                                  uuid: component.uuid,
                                  name: component.name,
                                }}
                                index={x.index}
                                primeSurface={x.primeSurface}
                              />,
                            ]
                          : []
                      )
                      .concat(
                        x.canCurrentUserRemoveEdge
                          ? [
                              <Button
                                key="remove"
                                onClick={() =>
                                  removeComponentAssembly(
                                    x.node.uuid,
                                    component.uuid
                                  )
                                }
                                loading={removingComponentAssembly}
                              >
                                Remove
                              </Button>,
                            ]
                          : []
                      )}
                  >
                    <List.Item.Meta
                      title={
                        <Space>
                          <Link href={paths.component(x.node.uuid)} legacyBehavior>
                            {x.node.name}
                          </Link>
                          <div>
                            <Tag color="purple">Layer {x.index}</Tag>
                            <Tag color="volcano">
                              Prime Surface {x.primeSurface}
                            </Tag>
                          </div>
                        </Space>
                      }
                      description={x.node.description}
                    />
                  </List.Item>
                ))}
              </List>
            )}
          </Col>
        </Row>
      )}
      {(component.concretizationOf.edges.length >= 1 ||
        component.concretizationOf.canCurrentUserAddEdge ||
        component.generalizationOf.edges.length >= 1 ||
        component.generalizationOf.canCurrentUserAddEdge) && (
        <Row gutter={[16, 16]}>
          <Col flex={1}>
            {(component.concretizationOf.edges.length >= 1 ||
              component.concretizationOf.canCurrentUserAddEdge) && (
              <List
                header="Concretization Of"
                bordered={true}
                size="small"
                footer={
                  component.concretizationOf.canCurrentUserAddEdge && (
                    <AddGeneralizationOfComponent
                      concreteComponentId={component.uuid}
                    />
                  )
                }
              >
                {component.concretizationOf.edges.map((x) => (
                  <List.Item
                    key={x.node.uuid}
                    actions={([] as ReactNode[]).concat(
                      x.canCurrentUserRemoveEdge
                        ? [
                            <Button
                              key="remove"
                              onClick={() =>
                                removeComponentGeneralization(
                                  x.node.uuid,
                                  component.uuid
                                )
                              }
                              loading={removingComponentGeneralization}
                            >
                              Remove
                            </Button>,
                          ]
                        : []
                    )}
                  >
                    <List.Item.Meta
                      title={
                        <Link href={paths.component(x.node.uuid)} legacyBehavior>
                          {x.node.name}
                        </Link>
                      }
                      description={x.node.description}
                    />
                  </List.Item>
                ))}
              </List>
            )}
          </Col>
          <Col flex={1}>
            {(component.generalizationOf.edges.length >= 1 ||
              component.generalizationOf.canCurrentUserAddEdge) && (
              <List
                header="Generalization Of"
                bordered={true}
                size="small"
                footer={
                  component.generalizationOf.canCurrentUserAddEdge && (
                    <AddConcretizationOfComponent
                      generalComponentId={component.uuid}
                    />
                  )
                }
              >
                {component.generalizationOf.edges.map((x) => (
                  <List.Item
                    key={x.node.uuid}
                    actions={([] as ReactNode[]).concat(
                      x.canCurrentUserRemoveEdge
                        ? [
                            <Button
                              key="remove"
                              onClick={() =>
                                removeComponentGeneralization(
                                  component.uuid,
                                  x.node.uuid
                                )
                              }
                              loading={removingComponentGeneralization}
                            >
                              Remove
                            </Button>,
                          ]
                        : []
                    )}
                  >
                    <List.Item.Meta
                      title={
                        <Link href={paths.component(x.node.uuid)} legacyBehavior>
                          {x.node.name}
                        </Link>
                      }
                      description={x.node.description}
                    />
                  </List.Item>
                ))}
              </List>
            )}
          </Col>
        </Row>
      )}
      {(component.variantOf.edges.length >= 1 ||
        component.variantOf.canCurrentUserAddEdge) && (
        <Row gutter={[16, 16]}>
          <Col flex={1}>
            <List
              header="Variant Of"
              bordered={true}
              size="small"
              footer={
                component.variantOf.canCurrentUserAddEdge && (
                  <AddVariantOfComponent componentId={component.uuid} />
                )
              }
            >
              {component.variantOf.edges.map((x) => (
                <List.Item
                  key={x.node.uuid}
                  actions={([] as ReactNode[]).concat(
                    x.canCurrentUserRemoveEdge
                      ? [
                          <Button
                            key="remove"
                            onClick={() =>
                              removeComponentVariant(
                                component.uuid,
                                x.node.uuid
                              )
                            }
                            loading={removingComponentVariant}
                          >
                            Remove
                          </Button>,
                        ]
                      : []
                  )}
                >
                  <List.Item.Meta
                    title={
                      <Link href={paths.component(x.node.uuid)} legacyBehavior>
                        {x.node.name}
                      </Link>
                    }
                    description={x.node.description}
                  />
                </List.Item>
              ))}
            </List>
          </Col>
        </Row>
      )}
    </Space>
  </>;
}
