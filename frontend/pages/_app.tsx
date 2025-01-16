import { AppProps } from "next/app";
import { ApolloProvider } from "@apollo/client";
import { useApollo } from "../lib/apollo";
import { CookiesProvider } from "react-cookie";
import '../styles/global.css';
import { ConfigProvider, message } from "antd";

export default function App({ Component, pageProps }: AppProps) {
  const apolloClient = useApollo(pageProps.initialApolloState);

  message.config({
    top: 100,
    duration: 2,
  });

  return (
    <ConfigProvider >
    <ApolloProvider client={apolloClient}>
      <CookiesProvider>
        <Component {...pageProps} />
      </CookiesProvider>
    </ApolloProvider>
    </ConfigProvider>
  );
}
