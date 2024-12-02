import { messageApolloError } from "../../lib/apollo";
import { useRouter } from "next/router";
import { Skeleton, Row, Col } from "antd";
import Layout from "../../components/Layout";
import paths, { redirectToLoginPage } from "../../paths";
import { useEffect } from "react";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import CreateApplication from "../../components/applications/CreateApplication";

function Page() {
  const router = useRouter();

  const { loading, data, error } = useCurrentUserQuery();
  const currentUser = data?.currentUser;
  const shouldRedirect = !(loading || error || currentUser);

  useEffect(() => {
    if (router.isReady && shouldRedirect) {
      redirectToLoginPage(router, paths.applicationCreate);
    }
  }, [shouldRedirect, router]);

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading || !currentUser) {
    // TODO Handle this case properly.
    return (
      <Layout>
        <Skeleton active avatar title />
      </Layout>
    );
  }

  return (
    <Layout>
      <Row justify="center">
        <Col flex={1}></Col>
        <Col flex={3}>
          <h2>
          Create Application
          </h2>
            <CreateApplication />
        </Col>
        <Col flex={1}></Col>
      </Row>
    </Layout>
  );
}

export default Page;