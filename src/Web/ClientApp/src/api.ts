export interface Result {
    isError: boolean;
    message?: string;
}

export const post = async (url: string, body: any) => {
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body),
    });

    try {
        const json = await response.json() as Promise<Result>;
        return json;
    } catch (err) {
        console.log(err);
        return { isError: true, message: 'An unexpected error occurred' };
    }
}