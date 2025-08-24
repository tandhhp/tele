import { apiNotificationCount } from "@/services/notification"
import { BellOutlined } from "@ant-design/icons"
import { history, useRequest } from "@umijs/max"
import { Badge } from "antd"

const NotificationBadge: React.FC = () => {

    const { data } = useRequest(apiNotificationCount);

    return (
        <Badge count={data}>
            <div className="w-6 h-6 flex items-center justify-center cursor-pointer hover:text-blue-500 transition-colors" onClick={() => history.push('/users/notification')}>
                <BellOutlined className="text-lg" />
            </div>
        </Badge>
    )
}

export default NotificationBadge;