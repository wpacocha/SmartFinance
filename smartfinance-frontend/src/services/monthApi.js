const API_URL = process.env.REACT_APP_API_URL;

export const getMonths = async (token) => {
    const res = await fetch("http://localhost:5201/api/month", {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
    return res.json();
};

export const createMonth = async (token, year, month) => {
    const res = await fetch("http://localhost:5201/api/month", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ Year: year, MonthNumber: month })
    });
    return res.json(); // lub return res.ok jeœli nie potrzebujesz danych
};

export const deleteMonth = async (token, id) => {
    const res = await fetch(`http://localhost:5201/api/month/${id}`, {
        method: "DELETE",
        headers: {
            Authorization: `Bearer ${token}`
        }
    });
    return res.ok;
};

