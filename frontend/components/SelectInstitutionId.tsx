import { Skeleton, Result } from "antd";
import { SearchSelect } from "./SearchSelect";
import { notEmpty } from "../lib/array";
import { useInstitutionsQuery } from "../queries/institutions.graphql";

export type SelectInstitutionIdProps<ValueType> = {
  mode?: "multiple" | "tags";
  value?: ValueType;
  onChange?: (value: ValueType) => void;
};

export function SelectInstitutionId<ValueType extends string>({
  mode,
  value,
  onChange,
}: SelectInstitutionIdProps<ValueType>) {
  // TODO Only fetch `name` and `uuid` because nothing more is needed.
  // TODO Use search instead of drop-down with all users/institutions preloaded. Be inspired by https://ant.design/components/select/#components-select-demo-select-users
  const { loading, data, error } = useInstitutionsQuery();
  const institutions = data?.institutions?.edges?.map((e) => e.node).filter(notEmpty);

  if (loading) {
    return <Skeleton />;
  }

  if (error) {
    console.error(error);
  }

  if (!institutions) {
    return <Result status="error" title="Failed to load institutions." />;
  }

  return (
    <SearchSelect
      value={value}
      mode={mode}
      onChange={onChange}
      options={
        institutions?.map((institution) => ({
          label: institution.name,
          value: institution.uuid,
        })) || []
      }
    />
  );
}
