const API_URL = process.env.REACT_APP_API_URL;

export async function getMonths(token) {
    const res = await fetch(`${API_URL}/month`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
    return await res.json();
}

export async function createMonth(token, year, month) {
    const res = await fetch(`${API_URL}/month`, {
        method: 'POST',
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ year, month })
    });
    return await res.json();
}

export async function deleteMonth(token, id) {
    await fetch(`${API_URL}/month/${id}`, {
        method: 'DELETE',
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
}
