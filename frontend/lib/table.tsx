import { List, Typography, Descriptions } from "antd";
import { SortOrder } from "antd/lib/table/interface";
import {
  doesFieldIncludeFilterValue,
  getFreeTextFilterProps,
} from "./freeTextFilter";
import Link from "next/link";
import {
  Publication,
  Scalars,
  Standard,
} from "../__generated__/__types__";
import { Highlight } from "../components/Highlight";
import paths from "../paths";

const sortDirections: SortOrder[] = ["ascend", "descend"];

function compare<ValueType>(
  x: ValueType | null | undefined,
  y: ValueType | null | undefined,
  nonNullCompare: (a: ValueType, b: ValueType) => number
) {
  if (x != null && y != null) {
    return nonNullCompare(x, y);
  }
  if (x == null && y != null) {
    return -1;
  }
  if (x != null && y == null) {
    return 1;
  }
  return 0;
}

function compareLexicographically<ValueType>(
  xs: (ValueType | null | undefined)[],
  ys: (ValueType | null | undefined)[],
  elementCompare: (x: ValueType, y: ValueType) => number
) {
  let i = 0;
  while (i < xs.length && i < ys.length) {
    const comparison = compare(xs[i], ys[i], elementCompare);
    if (comparison !== 0) {
      return comparison;
    }
    i++;
  }
  return xs.length - ys.length;
}

export function getColumnProps<RecordType>(
  title: string,
  key: keyof RecordType
) {
  return {
    title: title,
    key: key,
    // dataIndex: key,
  };
}

export function getDateTimeColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getValue: (record: RecordType) => Scalars["DateTime"] | null | undefined
) {
  return {
    ...getColumnProps(title, key),
    sorter: (a: RecordType, b: RecordType) =>
      compare(
        getValue(a),
        getValue(b),
        (x, y) => Date.parse(y) - Date.parse(x)
      ),
    sortDirections: sortDirections,
  };
}

export function getTimestampColumnProps<
  RecordType extends { timestamp: string }
>() {
  return {
    ...getDateTimeColumnProps<RecordType>(
      "Timestamp",
      "timestamp",
      (record) => record.timestamp
    ),
  };
}

export function getStringColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getValue: (record: RecordType) => string | null | undefined
) {
  return {
    ...getColumnProps(title, key),
    sorter: (a: RecordType, b: RecordType) =>
      compare(getValue(a), getValue(b), (x, y) => x.localeCompare(y, "en")),
    sortDirections: sortDirections,
  };
}

export function getFilterableStringColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getValue: (record: RecordType) => string | null | undefined,
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined,
  doRender: (
    record: RecordType,
    highlightedValue: JSX.Element,
    value: string | null | undefined
  ) => JSX.Element = (_record, highlightedValue, _value) => highlightedValue
) {
  return {
    ...getStringColumnProps(title, key, getValue),
    ...getFreeTextFilterProps<RecordType>(getValue, onFilterTextChange(key)),
    render: (_text: string, record: RecordType, _index: number) =>
      doRender(
        record,
        <Highlight text={getValue(record)} snippet={getFilterText(key)} />,
        getValue(record)
      ),
  };
}

export function getInternallyLinkedFilterableStringColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getValue: (record: RecordType) => string | null | undefined,
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined,
  getPath: (record: RecordType) => string
) {
  return getFilterableStringColumnProps(
    title,
    key,
    getValue,
    onFilterTextChange,
    getFilterText,
    (record, _highlightedValue, value) =>
      value ? (
        // TODO Why does this not work with `_highlightedValue`? An error is raised saying "Function components cannot be given refs. Attempts to access this ref will fail. Did you mean to use React.forwardRef()?": https://nextjs.org/docs/api-reference/next/link#if-the-child-is-a-function-component or https://reactjs.org/docs/forwarding-refs.html or https://deepscan.io/docs/rules/react-func-component-invalid-ref-prop or https://www.carlrippon.com/react-forwardref-typescript/
        (<Link href={getPath(record)} legacyBehavior>{value}</Link>)
      ) : (
        <></>
      )
  );
}

export function getExternallyLinkedFilterableLocatorColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getValue: (record: RecordType) => string | null | undefined,
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableStringColumnProps(
    title,
    key,
    getValue,
    onFilterTextChange,
    getFilterText,
    (record, highlightedValue, _value) => {
      const href = getValue(record);
      return href ? (
        <Typography.Link href={href}>{highlightedValue}</Typography.Link>
      ) : (
        <></>
      );
    }
  );
}

export function getUuidColumnProps<RecordType extends { uuid: string }>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined,
  getPath: (uuid: string) => string
) {
  return getInternallyLinkedFilterableStringColumnProps(
    "UUID",
    "uuid",
    (record) => record.uuid,
    onFilterTextChange,
    getFilterText,
    (record) => getPath(record.uuid)
  );
}

export function getNameColumnProps<RecordType extends { name?: string | null }>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableStringColumnProps(
    "Name",
    "name",
    (record) => record.name,
    onFilterTextChange,
    getFilterText
  );
}

export function getDescriptionColumnProps<
  RecordType extends { description?: string | null }
>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableStringColumnProps(
    "Description",
    "description",
    (record) => record.description,
    onFilterTextChange,
    getFilterText
  );
}

export function getAbbreviationColumnProps<
  RecordType extends { abbreviation?: string | null }
>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableStringColumnProps(
    "Abbreviation",
    "abbreviation",
    (record) => record.abbreviation,
    onFilterTextChange,
    getFilterText
  );
}

// TODO Use `EnumType extends enum` once there is an `enum` constraint as asked for in https://github.com/microsoft/TypeScript/issues/30611
export function getEnumListColumnProps<RecordType, EnumType extends string>(
  title: string,
  key: keyof RecordType,
  getValues: (record: RecordType) => EnumType[] | null | undefined
) {
  return {
    ...getColumnProps(title, key),
    sorter: (a: RecordType, b: RecordType) =>
      compare(getValues(a), getValues(b), (xs, ys) =>
        compareLexicographically(xs, ys, (x, y) => x.localeCompare(y, "en"))
      ),
    sortDirections: sortDirections,
  };
}

export function getFilterableEnumListColumnProps<
  RecordType,
  EnumType extends string
>(
  title: string,
  key: keyof RecordType,
  entries: [string, EnumType][],
  getValues: (record: RecordType) => EnumType[] | null | undefined,
  // TODO Call `onFilterTextChange` when the filter text changes (that is not done right now and that is the reason why matches are not highlighted). Note though that it cannot be called inside `onFilter` because that function is being called on render and we may not change state on render!
  _onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined,
  doRender: (
    record: RecordType,
    highlightedValue: JSX.Element,
    value: EnumType
  ) => JSX.Element = (_record, highlightedValue, _value) => highlightedValue
) {
  return {
    ...getEnumListColumnProps(title, key, getValues),
    filters: entries.map(([key, value]) => ({
      text: key,
      value: value,
    })),
    onFilter: (value: React.Key | boolean, record: RecordType) => {
      const values = getValues(record);
      if (
        typeof value === "string" &&
        entries
          .map(([_enumKey, enumValue]) => enumValue.toString())
          .includes(value)
      ) {
        return values?.includes(value as EnumType) || false;
      }
      return true;
    },
    render: (_value: string, record: RecordType, _index: number) => {
      const values = getValues(record);
      if (values == null) {
        return null;
      } else {
        return (
          <List>
            {values.map((value) => (
              <List.Item key={value}>
                {doRender(
                  record,
                  <Highlight text={value} snippet={getFilterText(key)} />,
                  value
                )}
              </List.Item>
            ))}
          </List>
        );
      }
    },
  };
}

export function getDescriptionListColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getEntries: (record: RecordType) =>
    | {
        key: string;
        title: string;
        value: string | null | undefined;
      }[]
    | null
    | undefined
) {
  return {
    ...getColumnProps(title, key),
    sorter: (a: RecordType, b: RecordType) => {
      const aValues = getEntries(a)?.map((x) => x.value);
      const bValues = getEntries(b)?.map((x) => x.value);
      return compare(aValues, bValues, (xs, ys) =>
        compareLexicographically(xs, ys, (x, y) => x.localeCompare(y, "en"))
      );
    },
    sortDirections: sortDirections,
  };
}

const titleValueDescriptionListSeparator = ": ";

function splitDescriptionListFilterText(filterText: string | null | undefined) {
  if (filterText == null) {
    return [null, null];
  }
  const filterTexts = filterText.split(titleValueDescriptionListSeparator);
  switch (filterTexts.length) {
    case 1:
      return [null, filterTexts[0]];
    case 2:
      return [filterTexts[0], filterTexts[1]];
    default:
      return [
        filterTexts[0],
        // TODO There are more efficient ways to do this than creating a new array without the first entry just to join them later on.
        filterTexts.slice(1, -1).join(titleValueDescriptionListSeparator),
      ];
  }
}

export function getFilterableDescriptionListColumnProps<RecordType>(
  title: string,
  key: keyof RecordType,
  getEntries: (record: RecordType) =>
    | {
        key: string;
        title: string;
        value: string | null | undefined;
        render?: (
          record: RecordType,
          highlightedValue: JSX.Element,
          value: string | null | undefined
        ) => JSX.Element;
      }[]
    | null
    | undefined,
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return {
    ...getDescriptionListColumnProps(title, key, getEntries),
    ...getFreeTextFilterProps<RecordType>(
      (record) =>
        getEntries(record)
          ?.map(
            ({ title, value }) =>
              `${title}${titleValueDescriptionListSeparator}${value}`
          )
          ?.join("\n"),
      onFilterTextChange(key)
    ),
    render: (_value: string, record: RecordType, _index: number) => {
      const titlesAndValues = getEntries(record);
      if (titlesAndValues == null) {
        return null;
      } else {
        return (
          <Descriptions column={1}>
            {titlesAndValues.map(
              ({
                key: entryKey,
                title,
                value,
                render = (_record, highlightedValue, _value) =>
                  highlightedValue,
              }) => {
                const [titleFilterText, valueFilterText] =
                  splitDescriptionListFilterText(getFilterText(key));
                return (
                  <Descriptions.Item
                    key={entryKey}
                    label={<Highlight text={title} snippet={titleFilterText} />}
                  >
                    {render(
                      record,
                      titleFilterText === null ||
                        doesFieldIncludeFilterValue(title, titleFilterText) ? (
                        <Highlight text={value} snippet={valueFilterText} />
                      ) : (
                        <Highlight text={value} snippet={null} />
                      ),
                      value
                    )}
                  </Descriptions.Item>
                );
              }
            )}
          </Descriptions>
        );
      }
    },
  };
}

export function getReferenceColumnProps<
  RecordType extends { reference?: Publication | Standard | null }
>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableDescriptionListColumnProps(
    "Reference",
    "reference",
    (record) =>
      record.reference && [
        {
          key: "title",
          title: "Title",
          value: record.reference.title,
        },
        {
          key: "abstract",
          title: "Abstract",
          value: record.reference.abstract,
        },
        {
          key: "section",
          title: "Section",
          value: record.reference.section,
        },
        ...(record.reference.__typename !== "Standard"
          ? []
          : [
              {
                key: "numeration",
                title: "Numeration",
                value: `${record.reference.numeration.prefix} ${record.reference.numeration.mainNumber} ${record.reference.numeration.suffix}`,
              },
              {
                key: "year",
                title: "Year",
                value: record.reference.year,
              },
              {
                key: "locator",
                title: "Locator",
                value: record.reference.locator,
                render: (
                  _record: RecordType,
                  hightlightedValue: JSX.Element,
                  value: string | null | undefined
                ) => (
                  // TODO Actually, `value` is neither `null` nor `undefined` but the type system does not know about it. How can we make it know about it so we don't need `|| ""` here?
                  (<Typography.Link href={value || ""}>
                    {hightlightedValue}
                  </Typography.Link>)
                ),
              },
              {
                key: "Standardizers",
                title: "Standardizers",
                value: record.reference.standardizers.join(", "),
              },
            ]),
        ...(record.reference.__typename !== "Publication"
          ? []
          : [
              {
                key: "arXiv",
                title: "arXiv",
                value: record.reference.arXiv,
              },
              {
                key: "doi",
                title: "DOI",
                value: record.reference.doi,
              },
              {
                key: "urn",
                title: "URN",
                value: record.reference.urn,
              },
              {
                key: "webAddress",
                title: "Website",
                value: record.reference.webAddress,
              },
              {
                key: "authors",
                title: "Authors",
                value: record.reference.authors?.join(", "),
              },
            ]),
      ],
    onFilterTextChange,
    getFilterText
  );
}

export function getComponentUuidColumnProps<
  RecordType extends { componentId: Scalars["Uuid"] }
>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getInternallyLinkedFilterableStringColumnProps(
    "Component UUID",
    "componentId",
    (x) => x.componentId,
    onFilterTextChange,
    getFilterText,
    (x) => paths.component(x.componentId)
  );
}

export function getAppliedMethodColumnProps<
  RecordType extends { appliedMethod: { methodId: Scalars["Uuid"] } }
>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableDescriptionListColumnProps(
    "Applied Method",
    "appliedMethod",
    (x) => [
      {
        key: "appliedMethodId",
        title: "UUID",
        value: x.appliedMethod.methodId,
        render: (_record, _highlightedValue, value) => (
          // TODO Why does this not work with `_highlightedValue`? An error is raised saying "Function components cannot be given refs. Attempts to access this ref will fail. Did you mean to use React.forwardRef()?": https://nextjs.org/docs/api-reference/next/link#if-the-child-is-a-function-component or https://reactjs.org/docs/forwarding-refs.html or https://deepscan.io/docs/rules/react-func-component-invalid-ref-prop or https://www.carlrippon.com/react-forwardref-typescript/
          (<Link href={paths.method(x.appliedMethod.methodId)} legacyBehavior>{value}</Link>)
        ),
      },
      // {
      //   key: "appliedMethodName",
      //   title: "Name",
      //   value: x.appliedMethod.method?.name,
      // },
      // {
      //   key: "appliedMethodDescription",
      //   title: "Description",
      //   value: x.appliedMethod.method?.description,
      // },
    ],
    onFilterTextChange,
    getFilterText
  );
}

export function getResourceTreeColumnProps<
  RecordType extends { resourceTree: { root: { value: { description: string, hashValue: string, locator: Scalars["Url"], dataFormatId: Scalars["Uuid"] }}} }
>(
  onFilterTextChange: (
    key: keyof RecordType
  ) => (newFilterText: string) => void,
  getFilterText: (key: keyof RecordType) => string | undefined
) {
  return getFilterableDescriptionListColumnProps(
    "Resource Tree Root",
    "resourceTree",
    (x) => [
      {
        key: "description",
        title: "Description",
        value: x.resourceTree.root.value.description,
      },
      {
        key: "hashValue",
        title: "Hash Value",
        value: x.resourceTree.root.value.hashValue,
      },
      {
        key: "locator",
        title: "Locator",
        value: x.resourceTree.root.value.locator,
        render: (_record, hightlightedValue, _value) => (
          <Typography.Link href={x.resourceTree.root.value.locator}>
            {hightlightedValue}
          </Typography.Link>
        ),
      },
      {
        key: "dataFormatId",
        title: "Data Format UUID",
        value: x.resourceTree.root.value.dataFormatId,
        render: (_record, _hightlightedValue, value) => (
          // TODO Why does this not work with `_highlightedValue`? An error is raised saying "Function components cannot be given refs. Attempts to access this ref will fail. Did you mean to use React.forwardRef()?": https://nextjs.org/docs/api-reference/next/link#if-the-child-is-a-function-component or https://reactjs.org/docs/forwarding-refs.html or https://deepscan.io/docs/rules/react-func-component-invalid-ref-prop or https://www.carlrippon.com/react-forwardref-typescript/
          (<Link
            href={paths.dataFormat(x.resourceTree.root.value.dataFormatId)}
            legacyBehavior>
            {value}
          </Link>)
        ),
      },
    ],
    onFilterTextChange,
    getFilterText
  );
}
