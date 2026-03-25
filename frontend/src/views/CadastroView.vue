<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import api from '@/services/api';

const router = useRouter();
const toast = useToast();

const nome = ref('');
const email = ref('');
const senha = ref('');
const carregando = ref(false);


const redirectCode = sessionStorage.getItem('returnToInvite');

const realizarCadastro = async () => {
    // Validação simples antes de enviar ao Back
    if (!nome.value || !email.value || !senha.value) {
        toast.add({ severity: 'warn', summary: 'Atenção', detail: 'Todos os campos são obrigatórios.', life: 3000 });
        return;
    }

    carregando.value = true;
    try {
        // Envia para o endpoint de registro do seu .NET
        await api.post('http://localhost:5000/api/auth/cadastro', {
            nome: nome.value,
            email: email.value,
            senha: senha.value
        });

        toast.add({ 
            severity: 'success', 
            summary: 'Conta Criada!', 
            detail: 'Agora você já pode fazer login.', 
            life: 3000 
        });

        // Redireciona para o login após o sucesso
        if (redirectCode) {
            sessionStorage.removeItem('returnToInvite');
            router.push(`/convite/${redirectCode}`);
        } else {
            router.push('/login');
        }
    } catch (error) {
        const msg = error.response?.data?.message || 'Erro ao criar conta. Tente outro e-mail.';
        toast.add({ severity: 'error', summary: 'Falha no Cadastro', detail: msg, life: 5000 });
    } finally {
        carregando.value = false;
    }
};
</script>

<template>
    <div class="flex align-items-center justify-content-center min-h-screen surface-ground p-4">
        <Card style="width: 100%; max-width: 450px" class="shadow-4">
            <template #title>
                <div class="text-center">
                    <h2 class="m-0">Crie sua Conta</h2>
                    <p class="text-600 text-sm mt-2">Comece a organizar seu Chá de Bebê hoje!</p>
                </div>
            </template>

            <template #content>
                <form @submit.prevent="realizarCadastro" class="flex flex-column gap-3">
                    <div class="flex flex-column gap-2">
                        <label for="nome" class="font-bold">Nome Completo</label>
                        <InputText id="nome" v-model="nome" placeholder="Como quer ser chamado?" />
                    </div>

                    <div class="flex flex-column gap-2">
                        <label for="email" class="font-bold">E-mail</label>
                        <InputText id="email" v-model="email" type="email" placeholder="seu@email.com" />
                    </div>

                    <div class="flex flex-column gap-2">
                        <label for="senha" class="font-bold">Senha</label>
                        <Password 
                            id="senha" 
                            v-model="senha" 
                            toggleMask 
                            placeholder="Crie uma senha forte"
                            promptLabel="Escolha uma senha"
                            weakLabel="Fraca"
                            mediumLabel="Média"
                            strongLabel="Forte"
                        />
                    </div>

                    <Button 
                        type="submit" 
                        label="Registrar" 
                        icon="pi pi-user-plus" 
                        :loading="carregando" 
                        class="w-full mt-3 p-button-success" 
                    />
                </form>
            </template>

            <template #footer>
                <div class="text-center">
                    <span>Já possui uma conta? </span>
                    <router-link to="/login" class="font-bold text-primary no-underline">Fazer Login</router-link>
                </div>
            </template>
        </Card>
    </div>
</template>

<style scoped>
/* Garante que o componente de senha ocupe 100% da largura */
:deep(.p-password input) {
    width: 100%;
}
</style>