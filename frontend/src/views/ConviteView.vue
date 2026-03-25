<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const router = useRouter();
const codigo = route.params.codigo;
const carregando = ref(false);

const participarDoEvento = async () => {
    // 1. Verificar se existe Token (Usuário logado)
    const token = localStorage.getItem('token');
    
    if (!token) {
        // Salva o código para retornar depois do login (opcional)
        sessionStorage.setItem('returnToInvite', codigo);
        router.push('/login');
        return;
    }
    var response;
    carregando.value = true;
    try {
        // 2. Envia a requisição de "Join" para o seu Backend C#
        response = await api.post(`http://localhost:5000/api/cha_de_bebe/entrar/${codigo}`);
        
        // 3. Se deu certo, leva o usuário para a lista de presentes do chá
        router.push('/dashboard'); 
    } catch (error) {
        // Tratar erro: convite expirado ou já participa
        console.error("Erro ao entrar no chá", error);
        const message = error.response?.data?.message || "Erro ao entrar no evento";
        console.error(message);
    } finally {
        carregando.value = false;
    }
};
</script>

<template>
    <div class="flex flex-column align-items-center justify-content-center min-h-screen p-4 surface-ground">
        <div class="surface-card p-6 shadow-2 border-round w-full max-w-25rem text-center">
            <i class="pi pi-envelope text-primary text-6xl mb-4"></i>
            <h1 class="text-2xl font-bold mb-2">Você foi convidado!</h1>
            <p class="text-600 mb-5">Clique no botão abaixo para visualizar a lista de presentes e participar deste Chá de Bebê.</p>
            
            <Button 
                label="Aceitar Convite e Entrar" 
                icon="pi pi-check" 
                class="w-full p-button-lg" 
                :loading="carregando"
                @click="participarDoEvento" 
            />
            
            <p class="mt-4 text-sm text-500">
                Ao entrar, o evento aparecerá no seu painel principal.
            </p>
        </div>
    </div>
</template>