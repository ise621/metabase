import { Skeleton, Result } from "antd";
import { SearchSelect } from "./SearchSelect";
import { notEmpty } from "../lib/array";
import { useComponentsQuery } from "../queries/components.graphql";

export type SelectComponentIdProps<ValueType> = {
  mode?: "multiple" | "tags";
  value?: ValueType;
  onChange?: (value: ValueType) => void;
};

export function SelectComponentId<ValueType extends string>({
  mode,
  value,
  onChange,
}: SelectComponentIdProps<ValueType>) {
  // TODO Only fetch `name` and `uuid` because nothing more is needed.
  // TODO Use search instead of drop-down with all users/components preloaded. Be inspired by https://ant.design/components/select/#components-select-demo-select-users
  const { loading, data, error } = useComponentsQuery();
  const components = data?.components?.edges?.map((e) => e.node).filter(notEmpty);

  if (loading) {
    return <Skeleton />;
  }

  if (error) {
    console.error(error);
  }

  if (!components) {
    return <Result status="error" title="Failed to load components." />;
  }

  return (
    <SearchSelect
      value={value}
      mode={mode}
      onChange={onChange}
      options={
        components?.map((component) => ({
          label: component.name,
          value: component.uuid,
        })) || []
      }
    />
  );
}
