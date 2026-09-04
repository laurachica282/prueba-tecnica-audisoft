import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
    issuer: 'http://localhost:8080/realms/school-management',
    redirectUri: window.location.origin,
    postLogoutRedirectUri: window.location.origin,
    clientId: 'school-web',
    responseType: 'code',
    scope: 'openid profile email',
    requireHttps: false,
    showDebugInformation: false,
    useSilentRefresh: false,
    clearHashAfterLogin: true,
    timeoutFactor: 0.75
};