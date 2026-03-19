import type { RecipeType } from "../types/RecipeTypes"
import { useContext, useEffect, useState } from "react"
import { AuthUserContext } from "../context/AuthenticatedUserContextProvider"
import { useMutation } from "@tanstack/react-query"
import { createLikeMutationOption, createUnlikeMutationOption } from "../mutationOptions/createRegisterMutationOption"
import styles from '../css/ForYou.module.css'

const Like = (recipe:RecipeType) => {
    const authUser = useContext(AuthUserContext)

    const [isLiked, setIsLiked] = useState<boolean>(recipe.likeolvaVan)
    const [likes, setLikes] = useState<number>(recipe.likes)

    const like = useMutation(createLikeMutationOption(recipe.id))
    const unlike = useMutation(createUnlikeMutationOption(recipe.id))

    const likeFunction = () => {
        if (authUser)
        {
            setIsLiked(!isLiked)
            setLikes(likes + 1)
            like.mutate()
        } else
            alert("Nincs bejelentkezve")
    }

    const unlikeFunction = () => {
        if (authUser)
        {
            setIsLiked(!isLiked)
            setLikes(likes - 1)
            unlike.mutate()
        }
    }

    useEffect(() => {
        setIsLiked(recipe.likeolvaVan)
        setLikes(recipe.likes)
    }, [recipe])

  return (
    <div className={styles.likes}>
        {likes}
        {isLiked ? 
            <i className="fa-solid fa-heart" onClick={() => {unlikeFunction()}}></i> 
        : 
            <i className="fa-regular fa-heart" onClick={() => {likeFunction()}}></i> 
        }
    </div>
  )
}

export default Like