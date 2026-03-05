import { mutationOptions } from "@tanstack/react-query";
import axios from "axios";

const BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms"
axios.defaults.headers.common['Authorization'] = `Bearer ${localStorage.getItem('access')}`;

export function createRegisterMutationOption(username: string, password: string) {
    return mutationOptions({
        mutationKey: ['register', username, password],
        mutationFn: () => postRegister(username, password)
    })
}

export function createCommentMutationOption(recipeId:string, comment:string){
    return mutationOptions({
        mutationKey: ['comment', comment],
        mutationFn: () => postComment(recipeId, comment)
    })
}

const postRegister = async (username: string, password: string) => {
    console.log("Try register...")
    const response = await axios.post(BASE_URL + "/api/Auth/register", {username, password})
    if(!response.data){
        console.error("Hibás felhasználó név vagy jelszó!");
        return
    }

    return await response.data
}

const postComment = async (recipeId:string, comment:string) => {
    console.log("Try comment...")
    const response = await axios.post(
        BASE_URL + `/api/Komment/recept/${recipeId}`, {szoveg: comment}
    )
    return await response.data
}