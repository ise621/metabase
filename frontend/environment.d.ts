declare global {
  namespace NodeJS {
    interface ProcessEnv {
      NEXT_PUBLIC_METABASE_URL: string;
      NEXT_WEBPACK_USEPOLLING: string;
      NODE_ENV: "test" | "development" | "production";
    }
  }
}
