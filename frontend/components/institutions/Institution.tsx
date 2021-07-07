import {
  Descriptions,
  Divider,
  List,
  Typography,
  message,
  Skeleton,
  Table,
} from "antd";
import {
  useInstitutionQuery,
  Scalars,
  Maybe,
  Numeration,
  Publication,
  Standard,
} from "../../queries/institutions.graphql";
import CreateComponent from "../components/CreateComponent";
import CreateMethod from "../methods/CreateMethod";
import CreateDataFormat from "../dataFormats/CreateDataFormat";
import CreateDatabase from "../databases/CreateDatabase";
import AddInstitutionRepresentative from "./AddInstitutionRepresentative";
import Link from "next/link";
import paths from "../../paths";
import { useEffect } from "react";

const renderReference = (
  reference:
    | Maybe<
        | ({ __typename?: "Publication" | undefined } & Pick<
            Publication,
            | "title"
            | "arXiv"
            | "authors"
            | "doi"
            | "urn"
            | "webAddress"
            | "abstract"
            | "section"
          >)
        | ({ __typename?: "Standard" | undefined } & Pick<
            Standard,
            | "title"
            | "abstract"
            | "section"
            | "locator"
            | "standardizers"
            | "year"
          > & {
              numeration: { __typename?: "Numeration" | undefined } & Pick<
                Numeration,
                "mainNumber" | "prefix" | "suffix"
              >;
            })
      >
    | undefined
) => (
  <Descriptions column={1}>
    {reference && (
      <>
        <Descriptions.Item label="Abstract">
          {reference?.abstract}
        </Descriptions.Item>
        <Descriptions.Item label="Section">
          {reference?.section}
        </Descriptions.Item>
        <Descriptions.Item label="Title">{reference?.title}</Descriptions.Item>
      </>
    )}
    {reference?.__typename === "Standard" && (
      <>
        <Descriptions.Item label="Locator">
          <Typography.Link href={reference.locator}>
            {reference.locator}
          </Typography.Link>
        </Descriptions.Item>
        <Descriptions.Item label="Numeration">{`${reference.numeration.prefix} ${reference.numeration.mainNumber} ${reference.numeration.suffix}`}</Descriptions.Item>
        <Descriptions.Item label="Standardizers">
          {reference.standardizers.join(", ")}
        </Descriptions.Item>
        <Descriptions.Item label="Year">{reference.year}</Descriptions.Item>
      </>
    )}
    {reference?.__typename === "Publication" && (
      <>
        <Descriptions.Item label="arXiv">{reference.arXiv}</Descriptions.Item>
        <Descriptions.Item label="Authors">
          {reference.authors?.join(", ")}
        </Descriptions.Item>
        <Descriptions.Item label="DOI">{reference.doi}</Descriptions.Item>
        <Descriptions.Item label="URN">{reference.urn}</Descriptions.Item>
        <Descriptions.Item label="Web Address">
          {reference.webAddress}
        </Descriptions.Item>
      </>
    )}
  </Descriptions>
);

export type InstitutionProps = {
  institutionId: Scalars["Uuid"];
};

export const Institution: React.FunctionComponent<InstitutionProps> = ({
  institutionId,
}) => {
  const { loading, error, data } = useInstitutionQuery({
    variables: {
      uuid: institutionId,
    },
  });
  const institution = data?.institution;

  useEffect(() => {
    if (error) {
      message.error(error);
    }
  }, [error]);

  if (loading || !institution) {
    return <Skeleton active avatar title />;
  }

  return (
    <>
      <Typography.Title>
        {institution.websiteLocator ? (
          <Typography.Link href={institution.websiteLocator}>
            {`${institution.name} (${institution.abbreviation})`}
          </Typography.Link>
        ) : (
          `${institution.name} (${institution.abbreviation})`
        )}
      </Typography.Title>
      <Typography.Text>{institution.description}</Typography.Text>
      <Divider />
      <Typography.Title level={2}>Manufactured Components</Typography.Title>
      <Table
        columns={[
          {
            title: "Name",
            dataIndex: "name",
            key: "name",
          },
          {
            title: "Abbreviation",
            dataIndex: "abbreviation",
            key: "abbreviation",
          },
          {
            title: "Description",
            dataIndex: "description",
            key: "description",
          },
          {
            title: "Categories",
            dataIndex: "categories",
            key: "categories",
            render: (categories) => categories.join(", "),
          },
          {
            title: "Availability",
            dataIndex: "availability",
            key: "availability",
            render: (availability) =>
              `${availability.from} to ${availability.to}`,
          },
        ]}
        dataSource={institution.manufacturedComponents.edges.map((x) => x.node)}
      />
      {institution.manufacturedComponents.canCurrentUserAddEdge ? (
        <CreateComponent manufacturerId={institution.uuid} />
      ) : (
        <></>
      )}
      <Divider />
      <Typography.Title level={2}>Operated Databases</Typography.Title>
      <Table
        columns={[
          {
            title: "Name",
            dataIndex: "name",
            key: "name",
          },
          {
            title: "Description",
            dataIndex: "description",
            key: "description",
          },
          {
            title: "Locator",
            dataIndex: "locator",
            key: "locator",
          },
        ]}
        dataSource={institution.operatedDatabases.edges.map((x) => x.node)}
      />
      {institution.operatedDatabases.canCurrentUserAddEdge ? (
        <CreateDatabase operatorId={institution.uuid} />
      ) : (
        <></>
      )}
      <Divider />
      <Typography.Title level={2}>Managed Data Formats</Typography.Title>
      <Table
        columns={[
          {
            title: "Name",
            dataIndex: "name",
            key: "name",
          },
          {
            title: "Extension",
            dataIndex: "extension",
            key: "extension",
          },
          {
            title: "Description",
            dataIndex: "description",
            key: "description",
          },
          {
            title: "Media Type",
            dataIndex: "mediaType",
            key: "mediaType",
          },
          {
            title: "Schema",
            dataIndex: "schemaLocator",
            key: "schemaLocator",
            render: (_text, row, _index) => (
              <Typography.Link href={row.schemaLocator}>
                {row.schemaLocator}
              </Typography.Link>
            ),
          },
          {
            title: "Reference",
            dataIndex: "reference",
            key: "reference",
            render: (_text, row, _index) => renderReference(row?.reference),
          },
        ]}
        dataSource={institution.managedDataFormats.edges.map((x) => x.node)}
      />
      {institution.managedDataFormats.canCurrentUserAddEdge ? (
        <CreateDataFormat managerId={institution.uuid} />
      ) : (
        <></>
      )}
      <Divider />
      <Typography.Title level={2}>Developed Methods</Typography.Title>
      <Table
        columns={[
          {
            title: "Name",
            dataIndex: "name",
            key: "name",
          },
          {
            title: "Description",
            dataIndex: "description",
            key: "description",
          },
          {
            title: "Validity",
            dataIndex: "validity",
            key: "validity",
            render: (_text, row, _index) =>
              `from ${row?.validity?.from} to ${row?.validity?.to}`,
          },
          {
            title: "Availability",
            dataIndex: "availability",
            key: "availability",
            render: (_text, row, _index) =>
              `from ${row?.availability?.from} to ${row?.availability?.to}`,
          },
          {
            title: "Reference",
            dataIndex: "reference",
            key: "reference",
            render: (_text, row, _index) => renderReference(row.reference),
          },
          {
            title: "Calculation Locator",
            dataIndex: "calculationLocator",
            key: "calculationLocator",
            render: (_text, row, _index) => (
              <Typography.Link href={row.calculationLocator}>
                {row.calculationLocator}
              </Typography.Link>
            ),
          },
          {
            title: "Categories",
            dataIndex: "categories",
            key: "categories",
            render: (_text, row, _index) => row.categories.join(", "),
          },
        ]}
        dataSource={institution.developedMethods.edges.map((x) => x.node)}
      />
      {institution.developedMethods.canCurrentUserAddEdge ? (
        <CreateMethod institutionDeveloperId={institution.uuid} />
      ) : (
        <></>
      )}
      <Divider />
      <Typography.Title level={2}>Representatives</Typography.Title>
      <List
        dataSource={institution.representatives.edges.map((x) => x.node)}
        renderItem={(item) => (
          <Link href={paths.user(item?.uuid)}>{item?.name}</Link>
        )}
      />
      {institution.representatives.canCurrentUserAddEdge ? (
        <AddInstitutionRepresentative institutionId={institution.uuid} />
      ) : (
        <></>
      )}
    </>
  );
};

export default Institution;
