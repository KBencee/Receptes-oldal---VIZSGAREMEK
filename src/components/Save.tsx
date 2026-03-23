import { useContext, useEffect, useState } from 'react'
import { AuthUserContext } from '../context/AuthenticatedUserContextProvider'
import type { RecipeType } from '../types/RecipeTypes'
import { useMutation } from '@tanstack/react-query'
import { createSaveMutationOption, createUnsaveMutationOption } from '../mutationOptions/createRegisterMutationOption'

const Save = (recipe:RecipeType) => {
    const authUser = useContext(AuthUserContext)
    const [isSaved, setIsSaved] = useState<boolean>(recipe.mentveVan)

    useEffect(()=>{
        setIsSaved(recipe.mentveVan)
    },[recipe])

    const save = useMutation(createSaveMutationOption(recipe.id))
    const unsave = useMutation(createUnsaveMutationOption(recipe.id))

    const saveFunction = () => {
        if (authUser)
        {
            save.mutate()
            setIsSaved(true)
        } else
            alert("Nincs bejelentkezve")
    }

    const unsaveFunction = () => {
        if (authUser)
        {
            unsave.mutate()
            setIsSaved(false)
        }
    }

  return (
    <div>
        {save.status == "pending" || unsave.status == "pending" ?
            <i className="fa-solid fa-spinner fa-spin"></i>
        :
            isSaved ?
                <i className="fa-solid fa-bookmark" onClick={() => {unsaveFunction()}}></i>
            :
                <i className="fa-regular fa-bookmark" onClick={() => {saveFunction()}}></i>
        }
    </div>
  )
}

export default Save