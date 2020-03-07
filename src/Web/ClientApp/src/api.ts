export interface Result {
    isError: boolean;
    message?: string;
}

export interface ResultObject<T> extends Result {
    response: T;
}

export interface Command {
    id: string;
    command: string;
}

export const queue = (url: string, body: any) => _post<Command>(url, body);
export const post = (url: string, body: any) => _post<Result>(url, body);
export const postResponse = <T>(url: string, body: any) => _post<ResultObject<T>>(url, body);

const _post = async <T>(url: string, body: any) => {
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body),
    });

    if (response.status == 500)
        throw {isError: true, message: await response.text() };

    try {
        const json = await response.json() as Promise<T>;
        return json;
    } catch (err) {
        console.log(err);
        throw { isError: true, message: 'An unexpected error occurred' };
    }
}