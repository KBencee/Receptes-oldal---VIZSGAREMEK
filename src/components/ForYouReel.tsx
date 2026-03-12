import { useMobileContext } from '../context/MobileContextProvider'
import HomeBtn from './HomeBtn'
import styles from '../css/ForYou.module.css'
import { createLikeMutationOption, createUnlikeMutationOption } from '../mutationOptions/createRegisterMutationOption'
import { useMutation } from '@tanstack/react-query'
import { useContext, useEffect, useState } from 'react'
import { AuthUserContext } from '../context/AuthenticatedUserContextProvider'
import { Link } from 'react-router-dom'
import type { RecipeType } from '../types/RecipeTypes'

const ForYouReel = (recipe:RecipeType) => {
    const {isMobile} = useMobileContext()
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
        }else
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
    <section className={isMobile ? styles.mobileSection : ""}>
        <div className={styles.forYouHead}><HomeBtn/><span>{recipe.nev}</span></div>
        <img src={recipe.kepUrl} />
        <aside>
            <div className={styles.likes}>
                {likes}
                {isLiked ? <i className="fa-solid fa-heart" onClick={() => {unlikeFunction()}}></i> : <i className="fa-regular fa-heart" onClick={() => {likeFunction()}}></i> }
            </div>
            <i className="fa-regular fa-bookmark"></i>
            <i className="fa-solid fa-share"></i>
            {isMobile ? <Link to={"/description/" + recipe.id}><i className="fa-solid fa-ellipsis"></i></Link> : ""}
        </aside>
        <div className={styles.tags}>
            {recipe.cimkek.map((c,idx) => <span key={idx} className={styles.tag}>{c}</span>)}
        </div>
    </section>
  )
}

export default ForYouReel