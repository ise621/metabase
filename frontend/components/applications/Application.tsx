import { useEffect, useState } from "react";
import { Scalars } from "../../__generated__/__types__";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import { useRouter } from "next/router";
import paths, { redirectToLoginPage } from "../../paths";
import { Col, Row, Skeleton, Tabs, TabsProps, Typography } from "antd";
import { messageApolloError } from "../../lib/apollo";
import UpdateApplication from "./UpdateApplication";
import AutorizationsTable from "../authorizations/AuthorizationsTable";
import TokenTable from "../tokens/TokenTable";

export type ApplicationProps = {
    applicationId: Scalars["Uuid"];
};

export default function Application({ applicationId }: ApplicationProps) {
    const { loading, error, data } = useCurrentUserQuery();
    const currentUser = data?.currentUser;
    const router = useRouter();
    const shouldRedirect = !(loading || error || currentUser);
    const [tab, setTab] = useState("application")

    useEffect(() => {
      window.addEventListener("hashchange", () => {
        const hash = window.location.hash.slice(1);
        setTab(hash);
      });
    }, []);
  
    const onChange = (key: string) => {
      setTab(key);
    };
  
    useEffect(() => {
      window.location.hash = tab;
    }, [tab]);

    useEffect(() => {
        if (router.isReady && shouldRedirect) {
            redirectToLoginPage(router, paths.application(applicationId));
        }
    }, [shouldRedirect, router]);

    useEffect(() => {
        if (error) {
            messageApolloError(error);
        }
    }, [error]);

    if (loading) {
        return <Skeleton active avatar title />;
    }

    const items: TabsProps['items'] = [
        {
            key: 'application',
            label: 'Application',
            children: <UpdateApplication applicationId={applicationId} />,
        },
        {
            key: 'authorization',
            label: 'Authorizations',
            children: <AutorizationsTable applicationId={applicationId}/>,
        },
        {
            key: 'token',
            label: 'Tokens',
            children: <TokenTable applicationId={applicationId}/>,
        },
    ];

    return (
        <>
        <Typography.Title>
          Edit Application
        </Typography.Title>
            <Row>
                <Col flex={1}></Col>
                <Col flex={3}>
                    <Tabs activeKey={tab} defaultActiveKey="application" items={items} onChange={onChange} />
                </Col>
                <Col flex={1}></Col>
            </Row>
        </>
    );
}