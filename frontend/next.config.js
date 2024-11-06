/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  swcMinify: true,
  webpack: (config, options) => {
    config.module.rules.push({
      test: /\.graphql$/,
      exclude: /node_modules/,
      use: [options.defaultLoaders.babel, { loader: "graphql-let/loader" }],
    });

    config.module.rules.push({
      test: /\.graphqls$/,
      exclude: /node_modules/,
      use: ["graphql-let/schema/loader"],
    });

    config.module.rules.push({
      test: /\.ya?ml$/,
      type: "json",
      use: "yaml-loader",
    });

    // Enable polling based on env variable being set
    // Inspired by https://medium.com/mikkotikkanen/solving-next-js-fast-refresh-on-docker-windows-71dfdb3ee785
    if (process.env.NEXT_WEBPACK_USEPOLLING == "true" || false) {
      config.watchOptions = {
        poll: 500,
        aggregateTimeout: 300
      }
    }

    return config;
  },
}

module.exports = nextConfig;
