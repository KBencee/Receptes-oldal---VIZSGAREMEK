import axios from "axios"

const BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms"

const protectedAPI = axios.create({baseURL: BASE_URL})

protectedAPI.interceptors.request.use(config =>{
    const token = localStorage.getItem("access")
    if (token){
        config.headers["Authorization"] = `Bearer ${token}`
    }
    return config
})

protectedAPI.interceptors.response.use(
    (response) => {return response},
    async (error) => {
        const originalRequest = error.config

        if(error.response?.status === 401 && !originalRequest.retryRequest){
            originalRequest.retryRequest = true
            try {
                const refreshToken = localStorage.getItem("refresh")
                const response = await axios.post(BASE_URL + "/api/Auth/refresh-token/",
                    {refresh: refreshToken})
                const newAccessToken = response.data.accessToken
                originalRequest.headers["Authorization"] = `Bearer ${newAccessToken}`
                localStorage.setItem("access", newAccessToken)
                console.log("Access token updated")
                return protectedAPI(originalRequest)
            } catch (error) {
                console.log(error)
                localStorage.clear()
            }
        }
        return Promise.reject(error)
    }
)

export async function getUserData() {
    try {
        const response = await protectedAPI.get("/api/Auth/me/")
        return response.data
    } catch (error) {
        console.error(error)
    }
}