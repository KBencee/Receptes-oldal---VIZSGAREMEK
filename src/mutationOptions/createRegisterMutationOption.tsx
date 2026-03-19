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

export function createLikeMutationOption(id: string) {
    return mutationOptions({
        mutationKey: ['like', id],
        mutationFn: () => postLike(id)
    })
}

export function createUnlikeMutationOption(id: string) {
    return mutationOptions({
        mutationKey: ['unlike', id],
        mutationFn: () => delLike(id)
    })
}
export function createSaveMutationOption(id: string) {
    return mutationOptions({
        mutationKey: ['save', id],
        mutationFn: () => postSave(id)
    })
}

export function createUnsaveMutationOption(id: string) {
    return mutationOptions({
        mutationKey: ['unSave', id],
        mutationFn: () => delSave(id)
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

const postLike = async (id: string) => {
    const response = await axios.post(BASE_URL + `/api/Recept/${id}/like`)
    return await response.data
}

const delLike = async (id: string) => {
    const response = await axios.delete(BASE_URL + `/api/Recept/${id}/like`)
    return await response.data
}

const postSave = async (id: string) => {
    const response = await axios.post(BASE_URL + `/saved/${id}`)
    return await response.data
}

const delSave = async (id: string) => {
    const response = await axios.delete(BASE_URL + `/saved/${id}`)
    return await response.data
}