import { Branch } from "@/utils/constants"

type Props = {
    branch: Branch;
}

const BranchTag: React.FC<Props> = ({ branch }) => {
    if (branch === Branch.South) {
        return (
            <div className="w-24 p-1 text-center bg-green-100 text-green-600 rounded text-[13px]">
                Hồ Chí Minh
            </div>
        )
    }
    return (
        <div className="w-24 p-1 text-center bg-orange-100 text-orange-600 rounded text-[13px]">
            Hà Nội
        </div>
    )
}

export default BranchTag;