import * as React from "react";
import Link from "next/link";
import { useRouter } from "next/router";
import { Menu, Button, Row, Flex } from "antd";
import { useCurrentUserQuery } from "../queries/currentUser.graphql";
import paths from "../paths";
import { getXsrfToken } from "../lib/apollo";
import { UserRole } from "../__generated__/__types__";
import { UserOutlined } from "@ant-design/icons";
import { Col } from "antd/lib";

type NavItemProps =
  | {
    path: string;
    label: string;
    subitems: null;
  }
  | { label: string; subitems: { path: string; label: string }[] };

export type NavBarProps = {
  items: NavItemProps[];
};

export default function NavBar({ items }: NavBarProps) {
  const router = useRouter();
  const currentUser = useCurrentUserQuery()?.data?.currentUser;

  return <>
    <Menu mode="horizontal" selectedKeys={[router.pathname]} theme="dark" >
      {items.map((item) =>
        item.subitems === null ? (
          <Menu.Item key={item.path}>
            <Link href={item.path} legacyBehavior>{item.label}</Link>
          </Menu.Item>
        ) : (
          // TODO find a better key
          (<Menu.SubMenu title={item.label} key={item.label}>
            {item.subitems.map((subitem) => (
              <Menu.Item key={subitem.path}>
                <Link href={subitem.path} legacyBehavior>{subitem.label}</Link>
              </Menu.Item>
            ))}
          </Menu.SubMenu>)
        )
      )}
      {/* I would like the following to be on the right but that is not possible at the moment, see issue https://github.com/ant-design/ant-design/issues/10749 */}
      {currentUser ? (
        <>
          {/* TODO Put information whether person is allowed to access OpenIdConnect information in query result of current user (using OpenIdConnectAuthorization) */}
          {currentUser?.canCurrentUserViewApplications && (
            <Menu.Item key={paths.openIdConnect}>
              <Link href={paths.openIdConnect}>OpenId Connect</Link>
            </Menu.Item>
          )}
          <Menu.SubMenu
            title={currentUser.name}
            key={paths.me.manage.home}
            icon={<UserOutlined />}
          >
            <Menu.Item key={paths.user(currentUser.uuid)}>
              <Link href={paths.user(currentUser.uuid)}>Profile</Link>
            </Menu.Item>
            <Menu.Item key={paths.me.manage.profile}>
              <Link href={paths.me.manage.profile}>Account</Link>
            </Menu.Item>
            <Menu.Item key={paths.openIdConnectClientLogout}>
              <form action={paths.openIdConnectClientLogout} method="post">
                <input
                  name="__RequestVerificationToken"
                  type="hidden"
                  value={
                    typeof window !== "undefined"
                      ? getXsrfToken() ?? undefined
                      : ""
                  }
                />
                <Button type="primary" htmlType="submit">
                  Logout
                </Button>
              </form>
            </Menu.Item>
          </Menu.SubMenu>
        </>
      ) : (
        <>
          <Menu.Item key={paths.openIdConnectClientLogin}>
            <Link href={paths.openIdConnectClientLogin}>Login</Link>
          </Menu.Item>
          <Menu.Item key={paths.userRegister}>
            <Link href={paths.userRegister}>Register</Link>
          </Menu.Item>
        </>
      )}
    </Menu>
  </>;
}
