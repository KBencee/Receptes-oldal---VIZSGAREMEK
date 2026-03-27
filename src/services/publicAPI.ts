

const BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms"

type TokenResponse = {
    accessToken: string,
    refreshToken: string
}

export async function loginUser(username: string, password: string) {
    console.log("Try login...");
    
    const response = await fetch(
        BASE_URL + "/api/Auth/login/",
        {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify({username, password})
        }
    )

    if(!response.ok){
        alert("Hibás felhasználó név vagy jelszó!");
        return
    }
    alert("Sikeres bejelentkezés")
    const data:TokenResponse = await response.json()
    console.log(data.accessToken)
    localStorage.setItem("access", data.accessToken)
    localStorage.setItem("refresh", data.refreshToken)
    return true
}