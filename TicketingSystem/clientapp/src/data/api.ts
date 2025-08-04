export const get = (url: string) => {
  return fetch(url).then((r) => r.json());
};

export const useApi = () => {
  const get = async <TResponse>(url: string) => {
    const response = await fetch(url);

    const data = (await response.json()) as TResponse;

    return data;
  };

  const post = async <TResponse>(url: string, body: unknown) => {
    const response = await fetch(url, {
      method: 'POST',
      body: JSON.stringify(body),
      headers: {
        'content-type': 'application/json',
      },
    });

    const data = (await response.json()) as TResponse;

    return data;
  };

  return { get, post };
};
