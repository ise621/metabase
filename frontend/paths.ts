export default {
  home: "/",
  antiforgeryToken: "/antiforgery/token",
  legalNotice: "/legal-notice",
  dataProtectionInformation: "/data-protection-information",
  databases: "/databases",
  database(uuid: string) {
    return `/databases/${encodeURIComponent(uuid)}`;
  },
  institutions: "/institutions",
  institution(uuid: string) {
    return `/institutions/${encodeURIComponent(uuid)}`;
  },
  institutionCreate: "/institutions/create",
  users: "/users",
  user(uuid: string) {
    return `/users/${encodeURIComponent(uuid)}`;
  },
  userCurrent: "me",
  me: {
    manage: {
      home: "/me/manage",
      profile: "/me/manage/profile",
      email: "/me/manage/email",
      changePassword: "/me/manage/change-password",
      setPassword: "/me/manage/set-password",
      twoFactorAuthentication: "/me/manage/two-factor-authentication",
      enableAuthenticator: "/me/manage/enable-authenticator",
      personalData: "/me/manage/personal-data",
    },
  },
  personalUserData: "/personal-user-data",
  openIdApplication(uuid: string) {
    return `/application/${encodeURIComponent(uuid)}`;
  },
  openIdConnectClientLogin: "https://local.buildingenvelopedata.org:4041/connect/client/login",
  openIdConnectClientLogout: "/connect/client/logout",
  userLogin: "/users/login",
  userRegister: "/users/register",
  userConfirmEmail: "/users/confirm-email",
  userForgotPassword: "/users/forgot-password",
  userLoginWithTwoFactorCode: "/users/login-with-two-factor-code",
  userLoginWithRecoveryCode: "/users/login-with-recovery-code",
  userCheckYourInboxAfterRegistration:
    "/users/check-your-inbox-after-registration",
  userCheckYourInboxAfterPasswordResetRequest:
    "/users/check-your-inbox-after-password-reset-request",
  dataFormats: "/data-formats",
  dataFormat(uuid: string) {
    return `/data-formats/${encodeURIComponent(uuid)}`;
  },
  methods: "/methods",
  method(uuid: string) {
    return `/methods/${encodeURIComponent(uuid)}`;
  },
  components: "/components",
  component(uuid: string) {
    return `/components/${encodeURIComponent(uuid)}`;
  },
  data: "/data",
  calorimetricData: "/data/calorimetric",
  hygrothermalData: "/data/hygrothermal",
  opticalData: "/data/optical",
  photovoltaicData: "/data/photovoltaic",
  geometricData: "/data/geometric",
  openIdConnect: "/openIdConnect",
};
