export interface IApiClientConfig {
    authToken?: string;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
class ApiClientBase {
    protected constructor(private config: IApiClientConfig) {}

    protected transformOptions = async (
        options: RequestInit
    ): Promise<RequestInit> => {
        if (!this.config.authToken) {
            return options;
        }

        return {
            ...options,
            headers: {
                ...options.headers,
                Authorization: this.config.authToken
            }
        };
    };
}
