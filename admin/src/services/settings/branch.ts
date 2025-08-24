import { request } from "@umijs/max";

export async function apiBranchOptions() {
    return request(`branch/options`);
}
