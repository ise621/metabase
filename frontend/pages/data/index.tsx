import Layout from "../../components/Layout";
import { Flex, List } from "antd";
import paths from "../../paths";
import Link from "next/link";

const navItems = [
  {
    path: paths.calorimetricData,
    label: "Calorimetric Data",
  },
  {
    path: paths.geometricData,
    label: "Geometric Data",
  },
  {
    path: paths.hygrothermalData,
    label: "Hygrothermal Data",
  },
  {
    path: paths.opticalData,
    label: "Optical Data",
  },
  {
    path: paths.photovoltaicData,
    label: "Photovoltaic Data",
  },
];

function Page() {
  return (
    <Layout>
      <Flex justify="center">
        <div style={{ maxWidth: 768 }}>
        <List
      bordered
      dataSource={navItems}
      renderItem={(item) => (
        <List.Item>
          <Link href={item.path} legacyBehavior>{item.label}</Link>
        </List.Item>
      )}
    />
        </div>
      </Flex>
    </Layout>
  );
}

export default Page;
