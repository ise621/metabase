import { messageApolloError } from "../../lib/apollo";
import Layout from "../../components/Layout";
import { useOpenIdConnectQuery } from "../../queries/openIdConnect.graphql";
import { useEffect } from "react";
import { Application } from "../../__generated__/__types__";
import { useCurrentUserQuery } from "../../queries/currentUser.graphql";
import ApplicationTable from "../../components/applications/ApplicationTable";
import { useRouter } from "next/router";
import paths, { redirectToLoginPage } from "../../paths";

function Page() {
  const { loading, error, data } = useOpenIdConnectQuery();
  const currentUser = useCurrentUserQuery()?.data?.currentUser;
  const router = useRouter();
  const shouldRedirect = !(loading || error || currentUser);

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  useEffect(() => {
    if (router.isReady && shouldRedirect) {
        redirectToLoginPage(router, paths.openIdConnect);
    }
}, [shouldRedirect, router]);

  return (
    <Layout>
      <ApplicationTable editable={currentUser?.canCurrentUserManageApplications} loading={loading} applications={data?.applications as Array<Application> || []}></ApplicationTable>
    </Layout>
  );
}

export default Page;
