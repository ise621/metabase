import { useRouter } from "next/router";
import Layout from "../../components/Layout";
import Application from "../../components/applications/Application";

function Page() {
  const router = useRouter();

  if (!router.isReady) {
    // Otherwise `uuid`, aka, `router.query`, is null on first render, see https://github.com/vercel/next.js/discussions/11484
    return null;
  }

  const { uuid } = router.query;

  return (
    <Layout>
      <Application applicationId={uuid} />
    </Layout>
  );
}

export default Page;